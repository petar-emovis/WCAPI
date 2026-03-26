using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WC.Models.DTO;

namespace WC.Service
{
    public interface IWcManagementService
    {
        Task<string> RefreshIpIntegersBinariesAndVersions();
        Task<string> RefreshIPv6HighsAndLows();
        Task<CountryResponse> GetCountryFromIpAdress(IpRangeRequest ipAddress);
    }
}
