using Microsoft.AspNetCore.Mvc;
using WC.Models.Admin.Dashboard;
using WC.Models.Admin.Import;
using WC.Service;

namespace WC.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImportsController : Controller
    {
        private readonly IWcManagementService _wcManagementService;

        public ImportsController(IWcManagementService wcManagementService)
        {
            _wcManagementService = wcManagementService;
        }

        [HttpPost(Name = "ImportIpRanges")]
        //public async Task<IActionResult> Index(ImportPageViewModel vm)
        //public async Task<ActionResult<ImportViewModel>> ImportIpRangesAsync([FromBody]ImportViewModel vm)
        public async Task<ActionResult<ImportResultModel>> ImportIpRangesAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            using var stream = file.OpenReadStream();
            var result = await _wcManagementService.ImportIpRangesAsync(stream, file.FileName);
            return Ok(result);

            ////return  await _wcManagementService.GetDashboardSummaryAsync();
            ////return View(model);

            //if (vm.File == null || vm.File.Length == 0)
            //{
            //    ModelState.AddModelError(nameof(vm.File), "Please select a file.");
            //    return View(vm);
            //}

            //try
            //{
            //    using var stream = vm.File.OpenReadStream();

            //    var result = await _wcManagementService.ImportIpRangesAsync(stream, vm.File.FileName);

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
