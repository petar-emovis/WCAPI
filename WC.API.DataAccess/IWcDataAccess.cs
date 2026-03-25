using DTO = WC.Models.DTO;

namespace WC.DataAccess
{
    public interface IWcDataAccess
    {
        Task<string> RefreshIpIntegersBinariesAndVersions();
        Task<DTO.CountryResponse> GetCountryFromIpAdress(string ipAdress);
    }
}
