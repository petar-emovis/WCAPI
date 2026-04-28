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
        Task<CountryResponse> GetCountryFromIpAddress(IpRangeRequest ipAddress);


        Task<IpRangePagedResultModel> GetIpRangesAsync(IpRangeFilterModel filter);
        Task<IpRangeViewModel?> GetIpRangeByIdAsync(int id);
        Task<List<CountryViewModel>> GetCountriesAsync();
        Task CreateIpRangeAsync(IpRangeViewModel model);
        Task UpdateIpRangeAsync(IpRangeViewModel model);
        Task DeleteIpRangeAsync(int id);

        Task<DashboardViewModel> GetDashboardSummaryAsync();
        Task<List<CountryViewModel>> GetCountryListAsync(string? search = null);

        Task<ImportResultModel> ImportIpRangesAsync(Stream stream, string fileName);
    }
}
