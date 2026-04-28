using Microsoft.AspNetCore.Mvc;
using WC.Admin.ApiClient;
using WC.Models.Admin.Import;
using WC.Service;

namespace WC.Admin.Controllers
{
    public class ImportsController : Controller
    {
        private readonly WcApiClient _wcApiClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public ImportsController(IHttpClientFactory httpClientFactory, IConfiguration config, WcApiClient wcApiClient)
        {
            _httpClientFactory = httpClientFactory;
            _wcApiClient = wcApiClient;
            _config = config;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ImportViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ImportViewModel vm)
        {
            if (vm.File == null || vm.File.Length == 0)
            {
                ModelState.AddModelError(nameof(vm.File), "Please select a file.");
                return View(vm);
            }

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiSettings:BaseUrl"]!);

            using var content = new MultipartFormDataContent();
            using var stream = vm.File.OpenReadStream();
            content.Add(new StreamContent(stream), "file", vm.File.FileName);

            var response = await client.PostAsync("/Imports", content);
            var result = await response.Content.ReadFromJsonAsync<ApiClient.ImportResultModel>();

            vm.Success = result?.Success ?? false;
            vm.ResultMessage = result?.Message;
            vm.ProcessedCount = result?.ProcessedCount ?? 0;
            vm.InsertedCount = result?.InsertedCount ?? 0;
            vm.SkippedCount = result?.SkippedCount ?? 0;

            return View(vm);

            //return View(await _wcApiClient.ImportIpRangesAsync(vm));

            //if (vm.File == null || vm.File.Length == 0)
            //{
            //    ModelState.AddModelError(nameof(vm.File), "Please select a file.");
            //    return View(vm);
            //}

            //try
            //{
            //    using var stream = vm.File.OpenReadStream();

            //    var result = await _wcApiClient.ImportIpRangesAsync(stream, vm.File.FileName);

            //    vm.ResultMessage = result.Message;
            //    vm.Success = result.Success;
            //    vm.ProcessedCount = result.ProcessedCount;
            //    vm.InsertedCount = result.InsertedCount;
            //    vm.UpdatedCount = result.UpdatedCount;
            //    vm.SkippedCount = result.SkippedCount;

            //    return View(vm);
            //}
            //catch (Exception ex)
            //{
            //    vm.Success = false;
            //    vm.ResultMessage = ex.Message;
            //    return View(vm);
            //}
        }
    }
}
