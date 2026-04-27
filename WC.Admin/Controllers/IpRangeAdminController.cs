using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WC.Admin.ApiClient;
using WC.Admin.Mappings;
using WC.Models;
using WC.Models.Admin;
using WC.Service;
using IpRangeViewModel = WC.Models.Admin.IpRangeViewModel;

namespace WC.Admin.Controllers
{
    public class IpRangeAdminController : Controller
    {
        //private readonly IWcManagementService _wcManagementService;
        private readonly WcApiClient _wcApiClient;

        public IpRangeAdminController(WcApiClient wcApiClient)
        {
            //_wcManagementService = wcManagementService;
            _wcApiClient = wcApiClient;
        }

        public async Task<IActionResult> Index(
            int? countryId, 
            int? ipVersion, 
            bool activeOnly = true,
            string? search = null,
            int page = 1,
            int pageSize = 50)
        {
            var model = await _wcApiClient.GetIpRangesAsync(
                 countryId,
                 ipVersion,
                activeOnly,
                search,
                page,
                pageSize
            );
            //var model = await _wcApiClient.GetIpRangesAsync(new ApiClient.IpRangeFilterModel
            //{
            //    CountryId = countryId,
            //    IpVersion = ipVersion,
            //    ActiveOnly = activeOnly,
            //    Search = search,
            //    Page = page,
            //    PageSize = pageSize
            //});

            return View(model.ToLocalModel());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = await BuildViewModelAsync(new IpRangeEditModel
            {
                Active = true,
                IpVersion = (int)IpVersionEnum.IPv4
            });

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IpRangeEditModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm = await BuildViewModelAsync(vm);
                return View(vm);
            }

            try
            {
                await _wcApiClient.CreateIpRangeAsync(new ApiClient.IpRangeViewModel
                {
                    CountryId = vm.CountryId,
                    IpVersion = vm.IpVersion,
                    StartIp = vm.StartIp,
                    EndIp = vm.EndIp,
                    Active = vm.Active
                });

                TempData["Success"] = "IP range created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                vm = await BuildViewModelAsync(vm);
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _wcApiClient.GetIpRangeByIdAsync(id);

            if (data == null)
                return NotFound();

            var vm = await BuildViewModelAsync(data.ToEditModel());

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IpRangeEditModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm = await BuildViewModelAsync(vm);
                return View(vm);
            }

            try
            {
                await _wcApiClient.UpdateIpRangeAsync(vm.Id, vm.ToApiModel());
                //await _wcApiClient.UpdateIpRangeAsync(vm.Id, new ApiClient.IpRangeViewModel
                //{
                //    Id = vm.Id,
                //    CountryId = vm.CountryId,
                //    IpVersion = vm.IpVersion,
                //    StartIp = vm.StartIp,
                //    EndIp = vm.EndIp,
                //    Active = vm.Active
                //});

                TempData["Success"] = "IP range updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                vm = await BuildViewModelAsync(vm);
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _wcApiClient.GetIpRangeByIdAsync(id);

            if (data == null)
                return NotFound();

            var vm = await BuildViewModelAsync(data.ToEditModel());

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _wcApiClient.DeleteIpRangeAsync(id);
            TempData["Success"] = "IP range deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<IpRangeEditModel> BuildViewModelAsync(IpRangeEditModel vm)
        {
            var countries = await _wcApiClient.GetCountriesAsync();

            vm.CountryOptions = countries
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToList();

            vm.IpVersionOptions = new List<SelectListItem>
            {
                new("IPv4", ((int)IpVersionEnum.IPv4).ToString()),
                new("IPv6", ((int)IpVersionEnum.IPv6).ToString())
            };

            return vm;
        }
    }
}
