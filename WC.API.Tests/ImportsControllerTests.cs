using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using WC.Admin.Controllers;
using WC.Admin.Models;
using WC.Models;
using WC.Models.Admin.Import;
using WC.Service;
using Xunit;

namespace WC.Admin.Tests.Controllers
{
    public class ImportsControllerTests
    {
        private readonly Mock<IWcManagementService> _serviceMock;
        private readonly ImportsController _controller;

        public ImportsControllerTests()
        {
            _serviceMock = new Mock<IWcManagementService>();

            _controller = new ImportsController(_serviceMock.Object);
            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>());
        }

        // ─── Index GET ────────────────────────────────────────────────────────────

        [Fact]
        public void Index_Get_ReturnsViewWithEmptyViewModel()
        {
            var result = _controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            Assert.IsType<ImportPageViewModel>(view.Model);            
        }

        // ─── Index POST ───────────────────────────────────────────────────────────

        [Fact]
        public async Task Index_Post_NullFile_ReturnsViewWithModelError()
        {
            var vm = new ImportPageViewModel { File = null };

            var result = await _controller.Index(vm);

            var view = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey(nameof(vm.File)));
            _serviceMock.Verify(s => s.ImportIpRangesAsync(It.IsAny<Stream>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Index_Post_EmptyFile_ReturnsViewWithModelError()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            var vm = new ImportPageViewModel { File = fileMock.Object };

            var result = await _controller.Index(vm);

            var view = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            _serviceMock.Verify(s => s.ImportIpRangesAsync(It.IsAny<Stream>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Index_Post_ValidFile_CallsServiceWithCorrectFileName()
        {
            var (vm, _) = BuildValidViewModel("ranges.csv");

            string? capturedFileName = null;
            _serviceMock
                .Setup(s => s.ImportIpRangesAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .Callback<Stream, string>((_, name) => capturedFileName = name)
                .ReturnsAsync(BuildImportResult());

            await _controller.Index(vm);

            Assert.Equal("ranges.csv", capturedFileName);
        }

        [Fact]
        public async Task Index_Post_SuccessfulImport_MapsResultToViewModel()
        {
            var (vm, _) = BuildValidViewModel();
            var importResult = BuildImportResult(
                success: true,
                message: "Import complete.",
                processed: 100,
                inserted: 80,
                updated: 15,
                skipped: 5);

            _serviceMock
                .Setup(s => s.ImportIpRangesAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(importResult);

            var result = await _controller.Index(vm);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ImportPageViewModel>(view.Model);

            Assert.True(model.Success);
            Assert.Equal("Import complete.", model.ResultMessage);
            Assert.Equal(100, model.ProcessedCount);
            Assert.Equal(80, model.InsertedCount);
            Assert.Equal(15, model.UpdatedCount);
            Assert.Equal(5, model.SkippedCount);
        }

        [Fact]
        public async Task Index_Post_FailedImport_MapsResultToViewModel()
        {
            var (vm, _) = BuildValidViewModel();
            var importResult = BuildImportResult(success: false, message: "Invalid format.");

            _serviceMock
                .Setup(s => s.ImportIpRangesAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(importResult);

            var result = await _controller.Index(vm);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ImportPageViewModel>(view.Model);

            Assert.False(model.Success);
            Assert.Equal("Invalid format.", model.ResultMessage);
        }

        [Fact]
        public async Task Index_Post_ServiceThrows_ReturnsViewWithErrorMessage()
        {
            var (vm, _) = BuildValidViewModel();

            _serviceMock
                .Setup(s => s.ImportIpRangesAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.Index(vm);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ImportPageViewModel>(view.Model);

            Assert.False(model.Success);
            Assert.Equal("Unexpected error", model.ResultMessage);
        }

        [Fact]
        public async Task Index_Post_ServiceThrows_DoesNotBubbleException()
        {
            var (vm, _) = BuildValidViewModel();

            _serviceMock
                .Setup(s => s.ImportIpRangesAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("boom"));

            var exception = await Record.ExceptionAsync(() => _controller.Index(vm));

            Assert.Null(exception);
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────

        private static (ImportPageViewModel vm, Mock<IFormFile> fileMock) BuildValidViewModel(
            string fileName = "test.csv")
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(128);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[128]));

            var vm = new ImportPageViewModel { File = fileMock.Object };
            return (vm, fileMock);
        }

        private static ImportResultModel BuildImportResult(
            bool success = true,
            string message = "OK",
            int processed = 0,
            int inserted = 0,
            int updated = 0,
            int skipped = 0) => new()
            {
                Success = success,
                Message = message,
                ProcessedCount = processed,
                InsertedCount = inserted,
                UpdatedCount = updated,
                SkippedCount = skipped
            };
    }
}