using Microsoft.AspNetCore.Mvc;
using WC.Admin.ApiClient;
using WC.Models.Admin.Country;
using WC.Service;

namespace WC.Admin.Controllers
{
    public class CountryController : Controller
    {
        private readonly WcApiClient _wcApiClient;

        public CountryController(WcApiClient wcApiClient)
        {
            _wcApiClient = wcApiClient;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var model = await _wcApiClient.GetCountriesFilteredAsync(search == null ? "" : search);
            return View(model);
        }
    }
}
