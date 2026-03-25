using System.Net;
using WC.DataAccess;
using WC.Models.DTO;
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

        public async Task<string> RefreshIpIntegersBinariesAndVersions()
        {
            return await _dataAccess.RefreshIpIntegersBinariesAndVersions();
        }

        public async Task<CountryResponse> GetCountryFromIpAdress(IpRangeRequest ipAddress)
        {
            //VALIDATE ipAdress
            var ip = IpParser.Parse(ipAddress.IpAddress);

            //if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            //{
            //    //IPv4

            //}
            //else if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            //{
            //    //IPv6

            //}

            var result = await _dataAccess.GetCountryFromIpAdress(ipAddress.IpAddress);

            return result;
        }
    }
}
