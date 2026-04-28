using Microsoft.AspNetCore.Mvc;
using WC.Models.Admin.Dashboard;
using WC.Service;

namespace WC.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DashboardController : Controller
    {
        private readonly IWcManagementService _wcManagementService;

        public DashboardController(IWcManagementService wcManagementService)
        {
            _wcManagementService = wcManagementService;
        }

        [HttpGet(Name = "GetDashboardSummary")]
        public async Task<ActionResult<DashboardViewModel>> Index()
        {
            return await _wcManagementService.GetDashboardSummaryAsync();
        }

        //može se i , bez injectiona
        //public async Task<ActionResult<DashboardViewModel>> Index([FromServices] IWcManagementService wcManagementService)
    }
}
