using Microsoft.AspNetCore.Mvc;
using WC.Admin.Models;
using WC.Service;

namespace WC.Admin.Controllers
{
    public class ImportsController : Controller
    {
        private readonly IWcManagementService _wcManagementService;

        public ImportsController(IWcManagementService wcManagementService)
        {
            _wcManagementService = wcManagementService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ImportPageViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ImportPageViewModel vm)
        {
            if (vm.File == null || vm.File.Length == 0)
            {
                ModelState.AddModelError(nameof(vm.File), "Please select a file.");
                return View(vm);
            }

            try
            {
                using var stream = vm.File.OpenReadStream();

                var result = await _wcManagementService.ImportIpRangesAsync(stream, vm.File.FileName);

                vm.ResultMessage = result.Message;
                vm.Success = result.Success;
                vm.ProcessedCount = result.ProcessedCount;
                vm.InsertedCount = result.InsertedCount;
                vm.UpdatedCount = result.UpdatedCount;
                vm.SkippedCount = result.SkippedCount;

                return View(vm);
            }
            catch (Exception ex)
            {
                vm.Success = false;
                vm.ResultMessage = ex.Message;
                return View(vm);
            }
        }
    }
}
