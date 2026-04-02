using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Sockets;
using WC.DataAccess.SqlServer.Configuration;
using WC.DataAccess.SqlServer.Map;
using WC.DataAccess.SqlServer.Models;
using WC.Models;
using WC.Models.Admin;
using WC.Models.Admin.Country;
using WC.Models.Admin.Dashboard;
using WC.Models.Admin.IpRange;
using WC.Models.DTO;
using WC.Models.Helpers;
using DTO = WC.Models.DTO;
using Entities = WC.DataAccess.SqlServer.Models;

namespace WC.DataAccess.SqlServer
{
    public class WcDataAccess : IWcDataAccess
    {
        private readonly WhichCountryContext _dbContext;
        private readonly IWcDataAccessConfiguration _configuration;
        private readonly DbContextOptionsBuilder<WhichCountryContext> _optionsBuilder;

        public  WcDataAccess(IWcDataAccessConfiguration configuration) 
        {
            _optionsBuilder = new();

            _optionsBuilder.UseSqlServer
                (
                    configuration.ConnectionString,
                    sqlServerOptions => sqlServerOptions.UseCompatibilityLevel(configuration.Timeout ?? 120)
                )
                .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
                .EnableSensitiveDataLogging();

            _configuration = configuration;
            _dbContext = new WhichCountryContext(_optionsBuilder.Options);            
        }

        public async Task<string> RefreshIpIntegersBinariesAndVersions()
        {
            var ipRanges = await _dbContext.IpRanges.ToListAsync();

            Parallel.ForEach(ipRanges.Where(I => I.IpVersion == null), range =>
            {
                if (IPAddress.TryParse(range.StartIp, out var startIp))
                {
                    if (range.IpVersion == null)
                        range.IpVersion = startIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? (int)IpVersionEnum.IPv4 : (int)IpVersionEnum.IPv6;

                    if (range.IpVersion == (int)IpVersionEnum.IPv4)
                        range.StartIpNumeric = HelperMethods.IpToLong(startIp);
                    else
                        range.StartIpBinary = startIp.GetAddressBytes();
                }

                if (IPAddress.TryParse(range.EndIp, out var endIp))
                {
                    if (range.IpVersion == null)
                        range.IpVersion = endIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? (int)IpVersionEnum.IPv4 : (int)IpVersionEnum.IPv6;

                    if (range.IpVersion == (int)IpVersionEnum.IPv4)
                        range.EndIpNumeric = HelperMethods.IpToLong(endIp);
                    else
                        range.EndIpBinary = endIp.GetAddressBytes();
                }
            });

            int a = await _dbContext.SaveChangesAsync();

            return "Initial refresh done! " + a.ToString();
        }

        public async Task<string> RefreshIPv6HighsAndLows()
        {
            var ipRanges = await _dbContext.IpRanges
                .Where(i=> i.IpVersion == (int)IpVersionEnum.IPv6
                && (i.StartIpv6High == null ||
                i.StartIpv6Low == null ||
                i.EndIpv6High == null ||
                i.EndIpv6Low == null))
                .ToListAsync();

            Parallel.ForEach(ipRanges, range =>
            {
                if (IPAddress.TryParse(range.StartIp, out var startIp))
                {
                    var (high, low) = HelperMethods.ToSqlOrderableParts(startIp);
                    range.StartIpv6High = high;
                    range.StartIpv6Low = low;
                }

                if (IPAddress.TryParse(range.EndIp, out var endIp))
                {
                    var (high, low) = HelperMethods.ToSqlOrderableParts(endIp);
                    range.EndIpv6High = high;
                    range.EndIpv6Low = low;
                }
            });

            int a = await _dbContext.SaveChangesAsync();

            return "IPv6 refresh done! " + a.ToString();
        }

        #region Get Country from Ip Address
        public async Task<DTO.CountryResponse> GetCountryFromIpAdress(string ipAddress, CancellationToken cancellationToken = default)
        {
            CountryResponse response = new DTO.CountryResponse();
            IPAddress ip = IPAddress.Parse(ipAddress);

            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                //IPv4
                long ipv4Value = HelperMethods.IpToLong(ip);

                var res = await FindByIpv4Async(ipv4Value, cancellationToken);

                if (res != null)
                    response = new CountryResponse { CountryName = res.Country.Name };

            }
            else //if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                //IPv6
                //BYTE VERSION

                //byte[] ipInBytes = ip.GetAddressBytes();

                //var ipv6List = await _dbContext.IpRanges
                //        .AsNoTracking()
                //        .Include(x => x.Country)
                //        .Where(i => i.IpVersion == (int)IpVersionEnum.IPv6).ToListAsync();

                //var res = ipv6List.Where(ip => HelperMethods.IsInRangeIpv6(ipInBytes, ip.StartIpBinary, ip.EndIpBinary)).FirstOrDefault();

                var (ipv6High, ipv6Low) = HelperMethods.ToSqlOrderableParts(ip);
                var res = await FindByIpv6Async(ipv6High, ipv6Low, cancellationToken);

                if (res != null)
                    response = new CountryResponse { CountryName = res.Country.Name };
            }

            return response;
        }

        private async Task<Entities.IpRange?> FindByIpv4Async(long ipv4Value, CancellationToken cancellationToken = default)
        {
            return await _dbContext.IpRanges
                    .AsNoTracking()
                    .Include(x => x.Country)
                    .Where(i => i.Active
                        && i.IpVersion == (int)IpVersionEnum.IPv4
                        && i.StartIpNumeric <= ipv4Value
                        && i.EndIpNumeric >= ipv4Value)
                    .OrderByDescending(i => i.StartIpNumeric)
                    .FirstOrDefaultAsync(cancellationToken);
        }
        private async Task<Entities.IpRange?> FindByIpv6Async(long ipv6High, long ipv6Low, CancellationToken cancellationToken = default)
        {
            return await _dbContext.IpRanges
                .AsNoTracking()
                .Include(x => x.Country)
                .Where(i => i.Active
                    && i.IpVersion == (int)IpVersionEnum.IPv6
                    && (
                        i.StartIpv6High < ipv6High
                        || (i.StartIpv6High == ipv6High && i.StartIpv6Low <= ipv6Low)
                    )
                    && (
                        i.EndIpv6High > ipv6High
                        || (i.EndIpv6High == ipv6High && i.EndIpv6Low >= ipv6Low)
                    ))
                .OrderByDescending(i => i.StartIpv6High)
                .ThenByDescending(i => i.StartIpv6Low)
                .FirstOrDefaultAsync(cancellationToken);
        }
        #endregion


        public IQueryable<DTO.IpRange> IpRangesAsNoTrackingWithCountryAsQueryable()
        {
            //var lista = _dbContext.IpRanges
            //    .AsNoTracking()
            //    .Include(x => x.Country)
            //    .AsQueryable();

            var lista = _dbContext.IpRanges
                .AsNoTracking()
                .ProjectTo<DTO.IpRange>(WC_Map.Mapper.ConfigurationProvider);


            return lista;
        }

        public async Task<IpRangePagedResultModel> GetIpRangesAsync(IpRangeFilterModel filter)
        {
            if (filter.Page <= 0)
                filter.Page = 1;

            if (filter.PageSize <= 0)
                filter.PageSize = 50;

            if (filter.PageSize > 200)
                filter.PageSize = 200;

            var query = _dbContext.IpRanges
               .AsNoTracking()
               .ProjectTo<DTO.IpRange>(WC_Map.Mapper.ConfigurationProvider);

            if (filter.CountryId.HasValue)
                query = query.Where(x => x.CountryId == filter.CountryId.Value);

            if (filter.IpVersion.HasValue)
                query = query.Where(x => (int?)x.IpVersion == filter.IpVersion.Value);

            if (filter.ActiveOnly)
                query = query.Where(x => x.Active);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();

                query = query.Where(x =>
                    (x.StartIp != null && x.StartIp.Contains(search)) ||
                    (x.EndIp != null && x.EndIp.Contains(search)));
                //(x.Country.Name != null && x.Country.Name.Contains(search)) ||
                //(x.Country.CountryCodeIso2 != null && x.Country.CountryCodeIso2.Contains(search)) ||
                //(x.Country.CountryCodeIso3 != null && x.Country.CountryCodeIso3.Contains(search)));
            }

            var totalCount = await query.CountAsync();

            //return await query
            var items = await query
                .OrderBy(x => x.Country.Name)
                //.ThenBy(x => x.StartIp) // Ovo jako USPORAVA!
                //.ThenBy(x => x.IpVersion)
                //.ThenBy(x => x.StartIpNumeric)
                //.ThenBy(x => x.StartIpv6High)
                //.ThenBy(x => x.StartIpv6Low)
                .Skip((filter.Page - 1) * filter.Page)
                .Take(filter.PageSize)
                .Select(x => new IpRangeViewModel
                {
                    Id = x.Id,
                    CountryId = x.CountryId,
                    CountryName = x.Country.Name,
                    CountryCodeIso2 = x.Country.CountryCodeIso2,
                    //CountryName = x.Country != null ? x.Country.Name : string.Empty,
                    //CountryCodeIso2 = x.Country != null ? x.Country.CountryCodeIso2 : string.Empty,
                    IpVersion = x.IpVersion != null ? (int)x.IpVersion : 0,
                    StartIp = x.StartIp ?? string.Empty,
                    EndIp = x.EndIp ?? string.Empty,
                    Active = x.Active
                })
                .ToListAsync();

            return new IpRangePagedResultModel
            {
                Items = items,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                CountryId = filter.CountryId,
                IpVersion = filter.IpVersion,
                ActiveOnly = filter.ActiveOnly,
                Search = filter.Search
            };
        }

        public async Task<IpRangeViewModel?> GetIpRangeByIdAsync(int id)
        {
            return await _dbContext.IpRanges
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new IpRangeViewModel
                {
                    Id = x.Id,
                    CountryId = x.CountryId,
                    CountryName = x.Country.Name,
                    IpVersion = x.IpVersion ?? 0,
                    StartIp = x.StartIp ?? string.Empty,
                    EndIp = x.EndIp ?? string.Empty,
                    Active = x.Active
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<CountryViewModel>> GetCountriesAsync()
        {
            return await _dbContext.Countries
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new CountryViewModel
                {
                    Id = x.Id,
                    Name = x.Name + " (" + x.CountryCodeIso2 + ")"
                })
                .ToListAsync();
        }

        public async Task CreateIpRangeAsync(DTO.IpRange model)
        {
            var entity = new Entities.IpRange();
            //MapAndNormalize(entity, model);
            entity = WC_Map.Mapper.Map<Entities.IpRange>(model);

            _dbContext.IpRanges.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateIpRangeAsync(DTO.IpRange model)
        {
            var entity = await _dbContext.IpRanges.FirstOrDefaultAsync(x => x.Id == model.Id);

            if (entity == null)
                throw new InvalidOperationException("IP range not found.");

            //entity = WC_Map.Mapper.Map<Entities.IpRange>(model);
            WC_Map.Mapper.Map(model, entity);

            int a = await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteIpRangeAsync(int id)
        {
            var entity = await _dbContext.IpRanges.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return;

            entity.Active = false;
            //_dbContext.IpRanges.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> GetTotalCountriesAsync()
        {
            return await _dbContext.Countries.CountAsync();
        }
        public async Task<int> GetTotalIpRangesAsync()
        {
            return await _dbContext.IpRanges.CountAsync();
        }
        public async Task<int> GetActiveIpRangesAsync()
        {
            return await _dbContext.IpRanges.CountAsync(x => x.Active);
        }
        public async Task<int> GetIpv4RangesAsync()
        {
            return await _dbContext.IpRanges.CountAsync(x => x.IpVersion == (int)IpVersionEnum.IPv4);
        }
        public async Task<int> GetIpv6RangesAsync()
        {
            return await _dbContext.IpRanges.CountAsync(x => x.IpVersion == (int)IpVersionEnum.IPv6);
        }

        public IQueryable<DTO.Country> CountriesAsNoTrackingAsQueryable()
        {
            //var query = _dbContext.Countries
            //    .AsNoTracking()
            //    .AsQueryable();

            var lista = _dbContext.Countries
                .AsNoTracking()
                .ProjectTo<DTO.Country>(WC_Map.Mapper.ConfigurationProvider);


            return lista;
        }

        public async Task<List<CountryViewModel>> GetCountryListAsync(IQueryable<DTO.Country> query)
        {
           return await query
                .OrderBy(x => x.Name)
                .Select(x => new CountryViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Iso2Code = x.CountryCodeIso2,
                    Iso3Code = x.CountryCodeIso3,
                    IpRangeCount = _dbContext.IpRanges.Count(r => r.CountryId == x.Id),
                    ActiveIpRangeCount = _dbContext.IpRanges.Count(r => r.CountryId == x.Id && r.Active)
                })
                .ToListAsync();
        }

        public async Task<DTO.Country?> GetCountryByCountryCodeAsync(string countryCode)
        {
            return await _dbContext.Countries
                .ProjectTo<DTO.Country>(WC_Map.Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.CountryCodeIso2 == countryCode);
                
        }

        public async Task<int> AddIpRangesAsync(List<DTO.IpRange> ipRanges)
        {
            foreach (var dtoModel in ipRanges)
            {
                Entities.IpRange entity = new Entities.IpRange();
                _dbContext.IpRanges.Add(WC_Map.Mapper.Map(dtoModel, entity));
            }

            return await _dbContext.SaveChangesAsync();
        }
    }
}
