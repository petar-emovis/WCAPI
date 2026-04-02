using WC.Models.Admin;
using WC.Models.Admin.Country;
using WC.Models.Admin.IpRange;
using WC.Models.DTO;
using DTO = WC.Models.DTO;

namespace WC.DataAccess
{
    public interface IWcDataAccess
    {
        Task<string> RefreshIpIntegersBinariesAndVersions();
        Task<string> RefreshIPv6HighsAndLows();
        Task<DTO.CountryResponse> GetCountryFromIpAdress(string ipAddress, CancellationToken cancellationToken = default);

        IQueryable<DTO.IpRange> IpRangesAsNoTrackingWithCountryAsQueryable();
        Task<IpRangePagedResultModel> GetIpRangesAsync(IpRangeFilterModel filter);
        Task<IpRangeViewModel?> GetIpRangeByIdAsync(int id);
        Task<List<CountryViewModel>> GetCountriesAsync();

        Task CreateIpRangeAsync(DTO.IpRange model);
        Task UpdateIpRangeAsync(DTO.IpRange model);
        Task DeleteIpRangeAsync(int id);

        Task<int> GetTotalCountriesAsync();
        Task<int> GetTotalIpRangesAsync();
        Task<int> GetActiveIpRangesAsync();
        Task<int> GetIpv4RangesAsync();
        Task<int> GetIpv6RangesAsync();
        IQueryable<DTO.Country> CountriesAsNoTrackingAsQueryable();
        Task<List<CountryViewModel>> GetCountryListAsync(IQueryable<DTO.Country> query);
        Task<DTO.Country?> GetCountryByCountryCodeAsync(string countryCode);
        Task<int> AddIpRangesAsync(List<DTO.IpRange> ipRanges);

    }
}
