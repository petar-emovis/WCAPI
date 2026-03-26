using Microsoft.EntityFrameworkCore;
using System.Net;
using WC.DataAccess.SqlServer.Configuration;
using WC.DataAccess.SqlServer.Helpers;
using WC.DataAccess.SqlServer.Models;
using WC.Models;
using WC.Models.DTO;
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

    }
}
