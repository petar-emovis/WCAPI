using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WC.DataAccess.SqlServer.Models;
using WC.Models.Admin;
using WC.Models.Admin.IpRange;
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

        [HttpGet(Name = "GetIpRanges")]
        public async Task<ActionResult<IpRangePagedResultModel>> GetAll([FromQuery] IpRangeFilterModel filter)
        => Ok(await _wcManagementService.GetIpRangesAsync(filter));

        [HttpGet("{id}", Name = "GetIpRangeById")]
        public async Task<ActionResult<IpRangeViewModel>> GetById(int id)
        {
            var result = await _wcManagementService.GetIpRangeByIdAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost(Name = "CreateIpRange")]
        public async Task<IActionResult> Create([FromBody] IpRangeViewModel model)
        {
            await _wcManagementService.CreateIpRangeAsync(model);
            return Ok();
        }

        [HttpPut("{id}", Name = "UpdateIpRange")]
        public async Task<IActionResult> Update(int id, [FromBody] IpRangeViewModel model)
        {
            model.Id = id;
            await _wcManagementService.UpdateIpRangeAsync(model);
            return Ok();
        }

        [HttpDelete("{id}", Name = "DeleteIpRange")]
        public async Task<IActionResult> Delete(int id)
        {
            await _wcManagementService.DeleteIpRangeAsync(id);
            return Ok();
        }





        //metode za prvotni unos podataka u bazu, nakon toga se vise nece koristiti
        //[HttpGet("[action]")]
        //public async Task<string> RefreshIpIntegersBinariesAndVersions()
        //{
        //    return await _wcManagementService.RefreshIpIntegersBinariesAndVersions();
        //}

        //[HttpGet("[action]")]
        //public async Task<string> RefreshIPv6HighsAndLows()
        //{
        //    return await _wcManagementService.RefreshIPv6HighsAndLows();
        //}
    }
}
