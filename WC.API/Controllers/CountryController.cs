using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using WC.Models.Admin;
using WC.Models.Admin.Country;
using WC.Models.Admin.IpRange;
using WC.Models.DTO;
using WC.Service;

namespace WC.API.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    [Route("[controller]/[action]")]
    public class CountryController : Controller
    {
        //private readonly ILogger<CountryController> _logger;
        private readonly IWcManagementService _wcManagementService;

        public CountryController(IWcManagementService wcManagementService)
        {
            //_logger = logger;
            _wcManagementService = wcManagementService;
        }

        [HttpGet(Name = "GetCountries")]
        public async Task<ActionResult<List<CountryViewModel>>> GetAll()
            => Ok(await _wcManagementService.GetCountryListAsync());

        [DisplayName("SearchCountries")]
        [HttpGet("{search}", Name = "GetCountriesFiltered")]
        //[HttpGet(Name = "GetCountriesFiltered")]
        public async Task<ActionResult<List<CountryViewModel>>> GetCountriesFiltered(string? search)
            => Ok(await _wcManagementService.GetCountryListAsync(search));


        [HttpGet("[action]", Name = "GetCountryFromIpAddress")]
        public async Task<CountryResponse> GetCountryFromIpAddress(string ipAddress)
        {
            return await _wcManagementService.GetCountryFromIpAddress(new IpRangeRequest { IpAddress = ipAddress });
        }

    }
}
