using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WC.Models.Admin;
using WC.Models.Admin.Country;
using WC.Models.Admin.Dashboard;
using WC.Models.Admin.Import;
using WC.Models.Admin.IpRange;
using WC.Models.DTO;

namespace WC.Service
{
    public interface IWcManagementService
    {
        Task<string> RefreshIpIntegersBinariesAndVersions();
        Task<string> RefreshIPv6HighsAndLows();
        Task<CountryResponse> GetCountryFromIpAdress(IpRangeRequest ipAddress);


        Task<IpRangePagedResultModel> GetIpRangesAsync(IpRangeFilterModel filter);
        Task<IpRangeEditModel?> GetIpRangeByIdAsync(int id);
        Task<List<CountryDropdownModel>> GetCountriesAsync();
        Task CreateIpRangeAsync(IpRangeEditModel model);
        Task UpdateIpRangeAsync(IpRangeEditModel model);
        Task DeleteIpRangeAsync(int id);

        Task<DashboardSummaryModel> GetDashboardSummaryAsync();
        Task<List<CountryListItemModel>> GetCountryListAsync(string? search = null);

        Task<ImportResultModel> ImportIpRangesAsync(Stream stream, string fileName);
    }
}
