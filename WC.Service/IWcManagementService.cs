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
        Task<CountryResponse> GetCountryFromIpAdress(IpRangeRequest ipAddress);
    }
}
