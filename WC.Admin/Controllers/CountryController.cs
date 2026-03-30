using Microsoft.AspNetCore.Mvc;
using WC.Models.Admin.Country;
using WC.Service;

namespace WC.Admin.Controllers
{
    public class CountryController : Controller
    {
        private readonly IWcManagementService _wcManagementService;

        public CountryController(IWcManagementService wcManagementService)
        {
            _wcManagementService = wcManagementService;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var model = await _wcManagementService.GetCountryListAsync(search);
            return View(model);
        }
    }
}
