using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WC.Models.Admin;
using WC.Service;

namespace WC.Admin.Controllers
{
    public class IpRangeAdminController : Controller
    {
        private readonly IWcManagementService _wcManagementService;

        public IpRangeAdminController(IWcManagementService wcManagementService)
        {
            _wcManagementService = wcManagementService;
        }

        public async Task<IActionResult> Index(
            int? countryId, 
            int? ipVersion, 
            bool activeOnly = true,
            string? search = null,
            int page = 1,
            int pageSize = 50)
        {
            var model = await _wcManagementService.GetIpRangesAsync(new IpRangeFilterModel
            {
                CountryId = countryId,
                IpVersion = ipVersion,
                ActiveOnly = activeOnly,
                Search = search,
                Page = page,
                PageSize = pageSize
            });

            //ViewBag.CountryId = countryId;
            //ViewBag.IpVersion = ipVersion;
            //ViewBag.ActiveOnly = activeOnly;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = await BuildViewModelAsync(new IpRangeEditPageViewModel
            {
                Active = true,
                IpVersion = 4
            });

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IpRangeEditPageViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm = await BuildViewModelAsync(vm);
                return View(vm);
            }

            try
            {
                await _wcManagementService.CreateIpRangeAsync(new IpRangeEditModel
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
            var data = await _wcManagementService.GetIpRangeByIdAsync(id);

            if (data == null)
                return NotFound();

            var vm = await BuildViewModelAsync(new IpRangeEditPageViewModel
            {
                Id = data.Id,
                CountryId = data.CountryId,
                IpVersion = data.IpVersion,
                StartIp = data.StartIp,
                EndIp = data.EndIp,
                Active = data.Active
            });

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IpRangeEditPageViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm = await BuildViewModelAsync(vm);
                return View(vm);
            }

            try
            {
                await _wcManagementService.UpdateIpRangeAsync(new IpRangeEditModel
                {
                    Id = vm.Id,
                    CountryId = vm.CountryId,
                    IpVersion = vm.IpVersion,
                    StartIp = vm.StartIp,
                    EndIp = vm.EndIp,
                    Active = vm.Active
                });

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
            var data = await _wcManagementService.GetIpRangeByIdAsync(id);

            if (data == null)
                return NotFound();

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _wcManagementService.DeleteIpRangeAsync(id);
            TempData["Success"] = "IP range deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<IpRangeEditPageViewModel> BuildViewModelAsync(IpRangeEditPageViewModel vm)
        {
            var countries = await _wcManagementService.GetCountriesAsync();

            vm.CountryOptions = countries
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToList();

            vm.IpVersionOptions = new List<SelectListItem>
            {
                new("IPv4", "4"),
                new("IPv6", "6")
            };

            return vm;
        }
    }
}
