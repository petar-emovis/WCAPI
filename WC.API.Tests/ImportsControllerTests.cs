using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using WC.API.Controllers;
using WC.Service;
using Xunit;

namespace WC.API.Tests.Controllers
{
    public class ImportsControllerTests
    {
        private readonly Mock<IWcManagementService> _serviceMock;
        private readonly ImportsController _controller;

        public ImportsControllerTests()
        {
            _serviceMock = new Mock<IWcManagementService>();
            _controller = new ImportsController(_serviceMock.Object);
        }

        // NullFile / EmptyFile

        [Fact]
        public async Task ImportIpRangesAsync_NullFile_ReturnsBadRequest()
        {
            var result = await _controller.ImportIpRangesAsync(null!);

            var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("No file provided.", bad.Value);
        }

        [Fact]
        public async Task ImportIpRangesAsync_EmptyFile_ReturnsBadRequest()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            var result = await _controller.ImportIpRangesAsync(fileMock.Object);

        }
    }
}