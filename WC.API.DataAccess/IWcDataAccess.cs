using WC.Models.DTO;
using DTO = WC.Models.DTO;

namespace WC.DataAccess
{
    public interface IWcDataAccess
    {
        Task<string> RefreshIpIntegersBinariesAndVersions();
        Task<string> RefreshIPv6HighsAndLows();
        Task<DTO.CountryResponse> GetCountryFromIpAdress(string ipAddress, CancellationToken cancellationToken = default);
    }
}
