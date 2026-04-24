using Microsoft.AspNetCore.Mvc;
using WC.Admin.ApiClient;
using WC.Models.Admin.Dashboard;
using WC.Service;

namespace WC.Admin.Controllers
{
    public class DashboardController : Controller
    {
        private readonly WcApiClient _wcApiClient;

        public DashboardController(WcApiClient wcApiClient)
        {
            _wcApiClient = wcApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _wcApiClient.GetDashboardSummaryAsync();
            return View(model);
        }
    }
}