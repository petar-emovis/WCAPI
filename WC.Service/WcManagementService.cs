using System.Net;
using System.Net.Sockets;
using System.Text;
using WC.DataAccess;
using WC.Models;
using WC.Models.Admin;
using WC.Models.Admin.Country;
using WC.Models.Admin.Dashboard;
using WC.Models.Admin.Import;
using WC.Models.Admin.IpRange;
using WC.Models.DTO;
using WC.Models.Helpers;
using WC.Service.Helper;

namespace WC.Service
{
    public class WcManagementService : IWcManagementService
    {

        private readonly IWcDataAccess _dataAccess;

        public WcManagementService(IWcDataAccess dataAccess) 
        { 
            _dataAccess = dataAccess;
        }

        #region Implementation
        public async Task<string> RefreshIpIntegersBinariesAndVersions()
        {
            return await _dataAccess.RefreshIpIntegersBinariesAndVersions();
        }
        public async Task<string> RefreshIPv6HighsAndLows()
        {
            return await _dataAccess.RefreshIPv6HighsAndLows();
        }

        public async Task<CountryResponse> GetCountryFromIpAdress(IpRangeRequest ipAddress)
        {
            //VALIDATE ipAdress
            if (!IPAddress.TryParse(ipAddress.IpAddress, out var ip))
                throw new InvalidOperationException("IP address is invalid.");

            var result = await _dataAccess.GetCountryFromIpAdress(ipAddress.IpAddress);

            return result;
        }

        public async Task<IpRangePagedResultModel> GetIpRangesAsync(IpRangeFilterModel filter)
        {
            //MsSqlResult 
            return await _dataAccess.GetIpRangesAsync(filter);
            //var PostGreSqlREsult = _dataAccess.GetIpv4RangesAsync(filter);

            ////BOLJE Premapirat modele
        }

        public async Task<IpRangeViewModel?> GetIpRangeByIdAsync(int id)
        {
            return await _dataAccess.GetIpRangeByIdAsync(id);
        }

        public async Task<List<CountryViewModel>> GetCountriesAsync()
        {
            return await _dataAccess.GetCountriesAsync();
        }

        public async Task CreateIpRangeAsync(IpRangeViewModel editModel)
        {
            var dtoModel = new IpRange();
            MapAndNormalize(dtoModel, editModel);

            await _dataAccess.CreateIpRangeAsync(dtoModel);
        }

        public async Task UpdateIpRangeAsync(IpRangeViewModel editModel)
        {
            var dtoModel = new IpRange();
            //var entity = await _dataAccess.IpRanges.FirstOrDefaultAsync(x => x.Id == model.Id);

            if (editModel == null)
                throw new InvalidOperationException("IP range not found.");

            MapAndNormalize(dtoModel, editModel);

            await _dataAccess.UpdateIpRangeAsync(dtoModel);
        }

        public async Task DeleteIpRangeAsync(int id)
        {
            //var dtoModel = new IpRange();
            //var entity = await _dataAccess.IpRanges.FirstOrDefaultAsync(x => x.Id == id);

            //if (entity == null)
            //    return;

            await _dataAccess.DeleteIpRangeAsync(id);
        }

        public async Task<DashboardViewModel> GetDashboardSummaryAsync()
        {

            var totalCountries = await _dataAccess.GetTotalCountriesAsync();
            var totalIpRanges = await _dataAccess.GetTotalIpRangesAsync();
            var activeIpRanges = await _dataAccess.GetActiveIpRangesAsync();
            var ipv4Ranges = await _dataAccess.GetIpv4RangesAsync();
            var ipv6Ranges = await _dataAccess.GetIpv6RangesAsync();

            return new DashboardViewModel
            {
                TotalCountries = totalCountries,
                TotalIpRanges = totalIpRanges,
                ActiveIpRanges = activeIpRanges,
                IPv4Ranges = ipv4Ranges,
                IPv6Ranges = ipv6Ranges,

                // set these once you have import history
                LastImportDate = null,
                LastImportStatus = null
            };
        }

        public async Task<List<CountryViewModel>> GetCountryListAsync(string? search = null)
        {
            var query = _dataAccess.CountriesAsNoTrackingAsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(search)) ||
                    (!string.IsNullOrEmpty(x.CountryCodeIso2) && x.CountryCodeIso2.Contains(search)) ||
                    (!string.IsNullOrEmpty(x.CountryCodeIso3) && x.CountryCodeIso3.Contains(search)));
            }

            return await _dataAccess.GetCountryListAsync(query);
        }

        public async Task<ImportResultModel> ImportIpRangesAsync(Stream stream, string fileName)
        {
            int processed = 0;
            int inserted = 0;
            int updated = 0;
            int skipped = 0;

            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: false);

            //bool isHeader = true;

            List<IpRange> ipRanges = new List<IpRange>();

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                //if (isHeader)
                //{
                //    isHeader = false;
                //    continue;
                //}

                processed++;

                var parts = line.Split(',');

                if (parts.Length < 3)
                {
                    skipped++;
                    continue;
                }

                string startIp = parts[0].Trim();
                string endIp = parts[1].Trim();
                string countryCode = parts[2].Trim();

                //var country = await _dataAccess.Countries
                //    .FirstOrDefaultAsync(x => x.Iso2Code == countryCode);
                var country = await _dataAccess.GetCountryByCountryCodeAsync(countryCode);

                if (country == null)
                {
                    skipped++;
                    continue;
                }

                int ipVersion = 0;
                int ipVersionEnd = 0;
                if (IPAddress.TryParse(startIp, out var ip))
                {
                    ipVersion = (ip.AddressFamily == AddressFamily.InterNetwork) ? (int)IpVersionEnum.IPv4 : (int)IpVersionEnum.IPv6;
                }
                else
                {
                    skipped++;
                    continue;
                }
                
                if (IPAddress.TryParse(endIp, out ip))
                {
                    ipVersionEnd = (ip.AddressFamily == AddressFamily.InterNetwork) ? (int)IpVersionEnum.IPv4 : (int)IpVersionEnum.IPv6;
                }
                else
                {
                    skipped++;
                    continue;
                }

                if (ipVersion != ipVersionEnd)
                {
                    //STARTIP i END IP SU RAZLIČITI, POGREŠAN JE REDAK
                    skipped++;
                    continue;
                }

                //DODAT VALIDACIJU ZA DUPLIKATE

                try
                {
                    //var entity = new WC.DataAccess.SqlServer.Models.IpRange();
                    var model = new IpRange();

                    MapAndNormalize(model, new IpRangeViewModel
                    {
                        CountryId = country.Id,
                        CountryName = country.Name,
                        IpVersion = ipVersion,
                        StartIp = startIp,
                        EndIp = endIp,
                        Active = true
                    });

                    ipRanges.Add(model);
                    //_dataAccess.IpRanges.Add(entity);
                    inserted++;
                }
                catch
                {
                    skipped++;
                }
            }


            int rowInsertNumber = await _dataAccess.AddIpRangesAsync(ipRanges);
            //await _dataAccess.SaveChangesAsync();
            //DOVRŠI rowInsertNumber

            return new ImportResultModel
            {
                Success = true,
                Message = $"Import completed from file '{fileName}'.",
                ProcessedCount = processed,
                InsertedCount = inserted,
                UpdatedCount = updated,
                SkippedCount = skipped
            };
        }
        #endregion

        #region Methods
        private static void MapAndNormalize(IpRange dtoModel, IpRangeViewModel editModel)
        {
            dtoModel.Id = editModel.Id;
            dtoModel.CountryId = editModel.CountryId;
            dtoModel.IpVersion = (IpVersionEnum)editModel.IpVersion;
            dtoModel.StartIp = editModel.StartIp.Trim();
            dtoModel.EndIp = editModel.EndIp.Trim();
            dtoModel.Active = editModel.Active;
            dtoModel.UpdateDate = DateTime.Now;

            if (dtoModel.CreationDate == default)
                dtoModel.CreationDate = DateTime.Now;

            if (!IPAddress.TryParse(editModel.StartIp, out var startIp))
                throw new InvalidOperationException("Start IP is invalid.");

            if (!IPAddress.TryParse(editModel.EndIp, out var endIp))
                throw new InvalidOperationException("End IP is invalid.");

            if (editModel.IpVersion == (int)IpVersionEnum.IPv4)
            {
                if (startIp.AddressFamily != AddressFamily.InterNetwork ||
                    endIp.AddressFamily != AddressFamily.InterNetwork)
                    throw new InvalidOperationException("Both IPs must be IPv4.");

                dtoModel.StartIpNumeric = HelperMethods.IpToLong(startIp);
                dtoModel.EndIpNumeric = HelperMethods.IpToLong(endIp);

                if (dtoModel.StartIpNumeric > dtoModel.EndIpNumeric)
                    throw new InvalidOperationException("Start IPv4 must be less than or equal to End IPv4.");

                //dtoModel.StartIpBinary = null;
                //dtoModel.EndIpBinary = null;
                dtoModel.StartIpv6High = null;
                dtoModel.StartIpv6Low = null;
                dtoModel.EndIpv6High = null;
                dtoModel.EndIpv6Low = null;
            }
            else if (editModel.IpVersion == (int)IpVersionEnum.IPv6)
            {
                if (startIp.AddressFamily != AddressFamily.InterNetworkV6 ||
                    endIp.AddressFamily != AddressFamily.InterNetworkV6)
                    throw new InvalidOperationException("Both IPs must be IPv6.");

                var (startHigh, startLow) = HelperMethods.ToSqlOrderableParts(startIp);
                var (endHigh, endLow) = HelperMethods.ToSqlOrderableParts(endIp);

                bool validRange = startHigh < endHigh
                    || (startHigh == endHigh && startLow <= endLow);

                if (!validRange)
                    throw new InvalidOperationException("Start IPv6 must be less than or equal to End IPv6.");

                dtoModel.StartIpNumeric = null;
                dtoModel.EndIpNumeric = null;
                dtoModel.StartIpBinary = startIp.GetAddressBytes();
                dtoModel.EndIpBinary = endIp.GetAddressBytes();
                dtoModel.StartIpv6High = startHigh;
                dtoModel.StartIpv6Low = startLow;
                dtoModel.EndIpv6High = endHigh;
                dtoModel.EndIpv6Low = endLow;
            }
            else
            {
                throw new InvalidOperationException("IP version must be 4 or 6.");
            }
        }
        #endregion


    }
}
