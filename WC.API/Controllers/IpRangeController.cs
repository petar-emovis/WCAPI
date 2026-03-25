using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WC.DataAccess.SqlServer.Models;
using WC.Models.DTO;
using WC.Service;

namespace WC.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class IpRangeController : Controller
    {
        private readonly ILogger<IpRangeController> _logger;
        private readonly IWcManagementService _wcManagementService;

        public IpRangeController(ILogger<IpRangeController> logger, IWcManagementService wcManagementService)
        {
            _logger = logger;
            _wcManagementService = wcManagementService;
        }

        //// GET: IpRangeController
        //public ActionResult Index()
        //{
        //    return View();
        //}

        [HttpGet("RefreshIp")]
        public async Task<string> RefreshIpIntegersBinariesAndVersions()
        {
            return await _wcManagementService.RefreshIpIntegersBinariesAndVersions();
        }

        
        [HttpGet("GetCountry/{ipAddress}")]
        public async Task<CountryResponse> GetCountry(string ipAddress)
        {
            return await _wcManagementService.GetCountryFromIpAdress(new IpRangeRequest { IpAddress = ipAddress });
        }

        // GET: IpRangeController/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: IpRangeController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: IpRangeController/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: IpRangeController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: IpRangeController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: IpRangeController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
