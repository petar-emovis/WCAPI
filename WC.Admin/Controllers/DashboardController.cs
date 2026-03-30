using Microsoft.AspNetCore.Mvc;
using WC.Admin.Models.Dashboard;
using WC.Service;

namespace WC.Admin.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IWcManagementService _wcManagementService;

        public DashboardController(IWcManagementService wcManagementService)
        {
            _wcManagementService = wcManagementService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _wcManagementService.GetDashboardSummaryAsync();
            return View(model);
        }
    }
}