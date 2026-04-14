using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WC.DataAccess.SqlServer.Models;
using WC.Models.DTO;
using WC.Service;

namespace WC.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IpRangeController : Controller
    {
        private readonly ILogger<IpRangeController> _logger;
        private readonly IWcManagementService _wcManagementService;

        public IpRangeController(ILogger<IpRangeController> logger, IWcManagementService wcManagementService)
        {
            _logger = logger;
            _wcManagementService = wcManagementService;
        }

        //[HttpGet("RefreshIp")]
        [HttpGet("[action]")]
        public async Task<string> RefreshIpIntegersBinariesAndVersions()
        {
            return await _wcManagementService.RefreshIpIntegersBinariesAndVersions();
        }

        [HttpGet("[action]")]
        public async Task<string> RefreshIPv6HighsAndLows()
        {
            return await _wcManagementService.RefreshIPv6HighsAndLows();
        }

        //[HttpGet("GetCountry/{ipAddress}")]
        [HttpGet("[action]")]
        public async Task<CountryResponse> GetCountryFromIpAdress(string ipAddress)
        {
            return await _wcManagementService.GetCountryFromIpAdress(new IpRangeRequest { IpAddress = ipAddress });
        }
    }
}
