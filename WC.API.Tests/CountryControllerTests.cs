using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using WC.Admin.Controllers;
using WC.Models.Admin.Country;
using WC.Service;
using Xunit;

namespace WC.Admin.Tests.Controllers
{
    public class CountryControllerTests
    {
        //private readonly Mock<IWcManagementService> _serviceMock;
        //private readonly CountryController _controller;

        //public CountryControllerTests()
        //{
        //    _serviceMock = new Mock<IWcManagementService>();

        //    _controller = new CountryController(_serviceMock.Object);
        //    _controller.TempData = new TempDataDictionary(
        //        new DefaultHttpContext(),
        //        Mock.Of<ITempDataProvider>());
        //}

        //[Fact]
        //public async Task Index_ReturnsViewWithModel()
        //{
        //    var expected = new List<CountryViewModel>();
        //    _serviceMock
        //        .Setup(s => s.GetCountryListAsync(It.IsAny<string?>()))
        //        .ReturnsAsync(expected);

        //    var result = await _controller.Index(null);

        //    var view = Assert.IsType<ViewResult>(result);
        //    Assert.Equal(expected, view.Model);
        //}

        //[Fact]
        //public async Task Index_PassesSearchTermToService()
        //{
        //    string? captured = null;
        //    _serviceMock
        //        .Setup(s => s.GetCountryListAsync(It.IsAny<string?>()))
        //        .Callback<string?>(s => captured = s)
        //        .ReturnsAsync(new List<CountryViewModel>());

        //    await _controller.Index("croatia");

        //    Assert.Equal("croatia", captured);
        //}

        //[Fact]
        //public async Task Index_NullSearch_PassesNullToService()
        //{
        //    string? captured = "not-null";
        //    _serviceMock
        //        .Setup(s => s.GetCountryListAsync(It.IsAny<string?>()))
        //        .Callback<string?>(s => captured = s)
        //        .ReturnsAsync(new List<CountryViewModel>());

        //    await _controller.Index(null);

        //    Assert.Null(captured);
        //}
    }
}