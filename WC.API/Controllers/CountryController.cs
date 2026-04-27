using Microsoft.AspNetCore.Mvc;
using WC.Models.Admin;
using WC.Models.Admin.Country;
using WC.Models.Admin.IpRange;
using WC.Models.DTO;
using WC.Service;

namespace WC.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountryController : Controller
    {
        private readonly IWcManagementService _wcManagementService;

        public CountryController(IWcManagementService wcManagementService)
        {
            //_logger = logger;
            _wcManagementService = wcManagementService;
        }

        [HttpGet(Name = "GetCountries")]
        public async Task<ActionResult<List<CountryViewModel>>> GetAll()
            => Ok(await _wcManagementService.GetCountryListAsync());

        [HttpGet("{search}", Name = "GetCountriesFiltered")]
        //[HttpGet(Name = "GetCountriesFiltered")]
        public async Task<ActionResult<List<CountryViewModel>>> GetCountriesFiltered(string? search)
            => Ok(await _wcManagementService.GetCountryListAsync(search));


        [HttpGet("[action]", Name = "GetCountryFromIpAdress")]
        public async Task<CountryResponse> GetCountryFromIpAdress(string ipAddress)
        {
            return await _wcManagementService.GetCountryFromIpAdress(new IpRangeRequest { IpAddress = ipAddress });
        }

    }
}
