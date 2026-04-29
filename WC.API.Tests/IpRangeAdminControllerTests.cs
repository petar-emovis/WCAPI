using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WC.API.Controllers;
using WC.Models.Admin;
using WC.Models.Admin.Country;
using WC.Models.Admin.IpRange;
using WC.Models.Admin.Import;
using WC.Models.DTO;
using WC.Service;
using Xunit;

namespace WC.API.Tests.Controllers
{
    // ─── IpRangeController ────────────────────────────────────────────────────

    public class IpRangeControllerTests
    {
        private readonly Mock<IWcManagementService> _serviceMock;
        private readonly Mock<ILogger<IpRangeController>> _loggerMock;
        private readonly IpRangeController _controller;

        public IpRangeControllerTests()
        {
            _serviceMock = new Mock<IWcManagementService>();
            _loggerMock = new Mock<ILogger<IpRangeController>>();
            _controller = new IpRangeController(_loggerMock.Object, _serviceMock.Object);
        }

        // GetAll

        [Fact]
        public async Task GetAll_ReturnsOkWithPagedResult()
        {
            var expected = new IpRangePagedResultModel();
            _serviceMock
                .Setup(s => s.GetIpRangesAsync(It.IsAny<IpRangeFilterModel>()))
                .ReturnsAsync(expected);

            var result = await _controller.GetAll(new IpRangeFilterModel());

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, ok.Value);
        }

        [Fact]
        public async Task GetAll_PassesFilterToService()
        {
            IpRangeFilterModel? captured = null;
            _serviceMock
                .Setup(s => s.GetIpRangesAsync(It.IsAny<IpRangeFilterModel>()))
                .Callback<IpRangeFilterModel>(f => captured = f)
                .ReturnsAsync(new IpRangePagedResultModel());

            var filter = new IpRangeFilterModel { Page = 2, PageSize = 25 };
            await _controller.GetAll(filter);

            Assert.Equal(2, captured!.Page);
            Assert.Equal(25, captured.PageSize);
        }

        // GetById

        [Fact]
        public async Task GetById_ExistingId_ReturnsOkWithModel()
        {
            var expected = new IpRangeViewModel { Id = 1 };
            _serviceMock.Setup(s => s.GetIpRangeByIdAsync(1)).ReturnsAsync(expected);

            var result = await _controller.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, ok.Value);
        }

        [Fact]
        public async Task GetById_NonExistingId_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetIpRangeByIdAsync(999)).ReturnsAsync((IpRangeViewModel?)null);

            var result = await _controller.GetById(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        // Create

        [Fact]
        public async Task Create_CallsServiceAndReturnsOk()
        {
            var model = new IpRangeViewModel { StartIp = "10.0.0.0", EndIp = "10.0.0.255" };
            _serviceMock.Setup(s => s.CreateIpRangeAsync(model)).Returns(Task.CompletedTask);

            var result = await _controller.Create(model);

            Assert.IsType<OkResult>(result);
            _serviceMock.Verify(s => s.CreateIpRangeAsync(model), Times.Once);
        }

        // Update

        [Fact]
        public async Task Update_SetsIdOnModelAndCallsService()
        {
            IpRangeViewModel? saved = null;
            _serviceMock
                .Setup(s => s.UpdateIpRangeAsync(It.IsAny<IpRangeViewModel>()))
                .Callback<IpRangeViewModel>(m => saved = m)
                .Returns(Task.CompletedTask);

            var model = new IpRangeViewModel { StartIp = "10.0.0.0" };
            var result = await _controller.Update(42, model);

            Assert.IsType<OkResult>(result);
            Assert.Equal(42, saved!.Id);
        }

        [Fact]
        public async Task Update_ReturnsOk()
        {
            _serviceMock
                .Setup(s => s.UpdateIpRangeAsync(It.IsAny<IpRangeViewModel>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.Update(1, new IpRangeViewModel());

            Assert.IsType<OkResult>(result);
        }

        // Delete

        [Fact]
        public async Task Delete_CallsServiceWithCorrectIdAndReturnsOk()
        {
            _serviceMock.Setup(s => s.DeleteIpRangeAsync(5)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(5);

            Assert.IsType<OkResult>(result);
            _serviceMock.Verify(s => s.DeleteIpRangeAsync(5), Times.Once);
        }
    }
}