using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using WC.API.Controllers;
using WC.Models.Admin.Country;
using WC.Models.DTO;
using WC.Service;
using Xunit;

namespace WC.API.Tests.Controllers
{
    public class CountryControllerTests
    {
        private readonly Mock<IWcManagementService> _serviceMock;
        private readonly CountryController _controller;

        public CountryControllerTests()
        {
            _serviceMock = new Mock<IWcManagementService>();
            _controller = new CountryController(_serviceMock.Object);
        }

        // GetAll

        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            var expected = new List<CountryViewModel> { new() { Id = 1, Name = "Croatia" } };
            _serviceMock.Setup(s => s.GetCountryListAsync(null)).ReturnsAsync(expected);

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, ok.Value);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithEmptyList()
        {
            _serviceMock.Setup(s => s.GetCountryListAsync(null)).ReturnsAsync(new List<CountryViewModel>());

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<List<CountryViewModel>>(ok.Value);
        }

        // GetCountriesFiltered

        [Fact]
        public async Task GetCountriesFiltered_PassesSearchTermToService()
        {
            string? captured = null;
            _serviceMock
                .Setup(s => s.GetCountryListAsync(It.IsAny<string?>()))
                .Callback<string?>(s => captured = s)
                .ReturnsAsync(new List<CountryViewModel>());

            await _controller.GetCountriesFiltered("cro");

            Assert.Equal("cro", captured);
        }

        [Fact]
        public async Task GetCountriesFiltered_ReturnsOkWithFilteredList()
        {
            var expected = new List<CountryViewModel> { new() { Name = "Croatia" } };
            _serviceMock
                .Setup(s => s.GetCountryListAsync("cro"))
                .ReturnsAsync(expected);

            var result = await _controller.GetCountriesFiltered("cro");

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, ok.Value);
        }

        [Fact]
        public async Task GetCountriesFiltered_NullSearch_PassesNullToService()
        {
            string? captured = "not-null";
            _serviceMock
                .Setup(s => s.GetCountryListAsync(It.IsAny<string?>()))
                .Callback<string?>(s => captured = s)
                .ReturnsAsync(new List<CountryViewModel>());

            await _controller.GetCountriesFiltered(null);

            Assert.Null(captured);
        }

        // GetCountryFromIpAddress

        [Fact]
        public async Task GetCountryFromIpAddress_ReturnsCountryResponse()
        {
            var expected = new CountryResponse { CountryName = "Croatia" };
            _serviceMock
                .Setup(s => s.GetCountryFromIpAddress(It.IsAny<IpRangeRequest>()))
                .ReturnsAsync(expected);

            var result = await _controller.GetCountryFromIpAddress("192.168.1.1");

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetCountryFromIpAddress_PassesCorrectIpToService()
        {
            IpRangeRequest? captured = null;
            _serviceMock
                .Setup(s => s.GetCountryFromIpAddress(It.IsAny<IpRangeRequest>()))
                .Callback<IpRangeRequest>(r => captured = r)
                .ReturnsAsync(new CountryResponse());

            await _controller.GetCountryFromIpAddress("10.0.0.1");

            Assert.Equal("10.0.0.1", captured!.IpAddress);
        }
    }
}