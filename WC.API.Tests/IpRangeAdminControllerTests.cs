using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Timers;
using WC.Admin;
using WC.Admin.Controllers;
using WC.Models;
using WC.Models.Admin;
using WC.Models.Admin.Country;
using WC.Models.Admin.IpRange;
using WC.Service;
using Xunit;

namespace WC.Admin.Tests.Controllers
{
    public class IpRangeAdminControllerTests
    {
        private readonly Mock<IWcManagementService> _serviceMock;
        private readonly IpRangeAdminController _controller;

        private static readonly List<CountryViewModel> FakeCountries = new()
        {
            new CountryViewModel { Id = 1, Name = "Croatia" },
            new CountryViewModel { Id = 2, Name = "Germany" }
        };

        public IpRangeAdminControllerTests()
        {
            _serviceMock = new Mock<IWcManagementService>();

            _serviceMock
                .Setup(s => s.GetCountriesAsync())
                .ReturnsAsync(FakeCountries);

            _controller = new IpRangeAdminController(_serviceMock.Object);

            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>());
        }

        // ─── Index ───────────────────────────────────────────────────────────────

        [Fact]
        public async Task Index_ReturnsViewWithModel()
        {
            var expected = new IpRangePagedResultModel();
            _serviceMock
                .Setup(s => s.GetIpRangesAsync(It.IsAny<IpRangeFilterModel>()))
                .ReturnsAsync(expected);

            var result = await _controller.Index(null, null, true, null, 1, 50);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(expected, view.Model);
        }

        [Fact]
        public async Task Index_PassesCorrectFilterToService()
        {
            IpRangeFilterModel? capturedFilter = null;
            _serviceMock
                .Setup(s => s.GetIpRangesAsync(It.IsAny<IpRangeFilterModel>()))
                .Callback<IpRangeFilterModel>(f => capturedFilter = f)
                .ReturnsAsync(new IpRangePagedResultModel());

            await _controller.Index(
                countryId: 5,
                ipVersion: 4,
                activeOnly: false,
                search: "192.168",
                page: 3,
                pageSize: 25);

            Assert.NotNull(capturedFilter);
            Assert.Equal(5, capturedFilter!.CountryId);
            Assert.Equal(4, capturedFilter.IpVersion);
            Assert.False(capturedFilter.ActiveOnly);
            Assert.Equal("192.168", capturedFilter.Search);
            Assert.Equal(3, capturedFilter.Page);
            Assert.Equal(25, capturedFilter.PageSize);
        }

        // ─── Create GET ───────────────────────────────────────────────────────────

        [Fact]
        public async Task Create_Get_IsDefaultActiveAndIpVersionSet()
        {
            var result = await _controller.Create();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<IpRangeEditModel>(view.Model);

            Assert.True(model.Active);
            Assert.Equal((int)IpVersionEnum.IPv4, model.IpVersion);
        }

        [Fact]
        public async Task Create_Get_IsCountryAndVersionOptionsPopulated()
        {
            var result = await _controller.Create();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<IpRangeEditModel>(view.Model);

            Assert.Equal(2, model.CountryOptions.Count);
            Assert.Equal(2, model.IpVersionOptions.Count);
            Assert.Contains(model.IpVersionOptions, o => o.Text == "IPv4");
            Assert.Contains(model.IpVersionOptions, o => o.Text == "IPv6");
        }

        // ─── Create POST ──────────────────────────────────────────────────────────

        [Fact]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            _serviceMock
                .Setup(s => s.CreateIpRangeAsync(It.IsAny<IpRangeViewModel>()))
                .Returns(Task.CompletedTask);

            var vm = BuildValidEditModel();

            var result = await _controller.Create(vm);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(IpRangeAdminController.Index), redirect.ActionName);
        }

        [Fact]
        public async Task Create_Post_ValidModel_CallsServiceWithCorrectData()
        {
            IpRangeViewModel? saved = null;
            _serviceMock
                .Setup(s => s.CreateIpRangeAsync(It.IsAny<IpRangeViewModel>()))
                .Callback<IpRangeViewModel>(v => saved = v)
                .Returns(Task.CompletedTask);

            var vm = BuildValidEditModel();

            await _controller.Create(vm);

            Assert.NotNull(saved);
            Assert.Equal(vm.CountryId, saved!.CountryId);
            Assert.Equal(vm.StartIp, saved.StartIp);
            Assert.Equal(vm.EndIp, saved.EndIp);
            Assert.Equal(vm.IpVersion, saved.IpVersion);
            Assert.Equal(vm.Active, saved.Active);
        }

        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsViewWithErrors()
        {
            _controller.ModelState.AddModelError("StartIp", "Required");

            var result = await _controller.Create(new IpRangeEditModel());

            var view = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            _serviceMock.Verify(s => s.CreateIpRangeAsync(It.IsAny<IpRangeViewModel>()), Times.Never);
        }

        [Fact]
        public async Task Create_Post_ServiceThrows_ReturnsViewWithModelError()
        {
            _serviceMock
                .Setup(s => s.CreateIpRangeAsync(It.IsAny<IpRangeViewModel>()))
                .ThrowsAsync(new Exception("Overlapping range"));

            var result = await _controller.Create(BuildValidEditModel());

            var view = Assert.IsType<ViewResult>(result);
            Assert.True(_controller.ModelState.ErrorCount > 0);
        }

        // ─── Edit GET ─────────────────────────────────────────────────────────────

        [Fact]
        public async Task Edit_Get_ExistingId_ReturnsViewWithModel()
        {
            var data = BuildFakeViewModel(id: 7);
            _serviceMock.Setup(s => s.GetIpRangeByIdAsync(7)).ReturnsAsync(data);

            var result = await _controller.Edit(7);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<IpRangeEditModel>(view.Model);
            Assert.Equal(7, model.Id);
            Assert.Equal(data.StartIp, model.StartIp);
            Assert.Equal(data.EndIp, model.EndIp);
        }

        [Fact]
        public async Task Edit_Get_NonExistingId_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetIpRangeByIdAsync(999)).ReturnsAsync((IpRangeViewModel?)null);

            var result = await _controller.Edit(999);

            Assert.IsType<NotFoundResult>(result);
        }

        // ─── Edit POST ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Edit_Post_ValidModel_RedirectsToIndex()
        {
            _serviceMock
                .Setup(s => s.UpdateIpRangeAsync(It.IsAny<IpRangeViewModel>()))
                .Returns(Task.CompletedTask);


            var result = await _controller.Edit(BuildValidEditModel(id: 7));

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(IpRangeAdminController.Index), redirect.ActionName);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_CallsServiceWithCorrectId()
        {
            IpRangeViewModel? updated = null;
            _serviceMock
                .Setup(s => s.UpdateIpRangeAsync(It.IsAny<IpRangeViewModel>()))
                .Callback<IpRangeViewModel>(v => updated = v)
                .Returns(Task.CompletedTask);

            await _controller.Edit(BuildValidEditModel(id: 42));

            Assert.NotNull(updated);
            Assert.Equal(42, updated!.Id);
        }

        [Fact]
        public async Task Edit_Post_InvalidModel_ReturnsViewWithoutCallingService()
        {
            _controller.ModelState.AddModelError("EndIp", "Required");

            var result = await _controller.Edit(new IpRangeEditModel());

            Assert.IsType<ViewResult>(result);
            _serviceMock.Verify(s => s.UpdateIpRangeAsync(It.IsAny<IpRangeViewModel>()), Times.Never);
        }

        [Fact]
        public async Task Edit_Post_ServiceThrows_ReturnsViewWithModelError()
        {
            _serviceMock
                .Setup(s => s.UpdateIpRangeAsync(It.IsAny<IpRangeViewModel>()))
                .ThrowsAsync(new Exception("Conflict"));

            var result = await _controller.Edit(BuildValidEditModel(id: 7));

            Assert.IsType<ViewResult>(result);
            Assert.True(_controller.ModelState.ErrorCount > 0);
        }

        // ─── Delete GET ───────────────────────────────────────────────────────────

        [Fact]
        public async Task Delete_Get_ExistingId_ReturnsViewWithModel()
        {
            _serviceMock.Setup(s => s.GetIpRangeByIdAsync(3)).ReturnsAsync(BuildFakeViewModel(id: 3));

            var result = await _controller.Delete(3);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<IpRangeEditModel>(view.Model);
            Assert.Equal(3, model.Id);
        }

        [Fact]
        public async Task Delete_Get_NonExistingId_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetIpRangeByIdAsync(404)).ReturnsAsync((IpRangeViewModel?)null);

            var result = await _controller.Delete(404);

            Assert.IsType<NotFoundResult>(result);
        }

        // ─── DeleteConfirmed POST ─────────────────────────────────────────────────

        [Fact]
        public async Task DeleteConfirmed_CallsServiceAndRedirects()
        {
            _serviceMock
                .Setup(s => s.DeleteIpRangeAsync(5))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteConfirmed(5);

            _serviceMock.Verify(s => s.DeleteIpRangeAsync(5), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(IpRangeAdminController.Index), redirect.ActionName);
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────

        private static IpRangeEditModel BuildValidEditModel(int id = 0) => new()
        {
            Id = id,
            CountryId = 1,
            IpVersion = (int)IpVersionEnum.IPv4,
            StartIp = "192.168.1.0",
            EndIp = "192.168.1.255",
            Active = true
        };

        private static IpRangeViewModel BuildFakeViewModel(int id) => new()
        {
            Id = id,
            CountryId = 1,
            CountryName = "Croatia",
            IpVersion = (int)IpVersionEnum.IPv4,
            StartIp = "10.0.0.0",
            EndIp = "10.0.0.255",
            Active = true
        };
    }
}