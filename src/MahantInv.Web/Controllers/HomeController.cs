using AutoMapper;
using MahantInv.Infrastructure.Interfaces;
using MahantInv.Web.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MahantInv.Web.Controllers
{
    /// <summary>
    /// A sample MVC controller that uses views.
    /// Razor Pages provides a better way to manage view-based content, since the behavior, viewmodel, and view are all in one place,
    /// rather than spread between 3 different folders in your Web project. Look in /Pages to see examples.
    /// See: https://ardalis.com/aspnet-core-razor-pages-%E2%80%93-worth-checking-out/
    /// </summary>
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductUsageRepository _productUsageRepository;

        public HomeController(ILogger<HomeController> logger, IProductUsageRepository productUsageRepository, IMapper mapper) : base(mapper)
        {
            _logger = logger;
            _productUsageRepository = productUsageRepository;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Sell()
        {
            try
            {
                var buyers = await _productUsageRepository.ListAllAsync();
                ViewBag.Buyers = new SelectList(buyers.Where(b => !string.IsNullOrWhiteSpace(b.Buyer)).Select(b => new { Buyer = b.Buyer }).Distinct(), "Buyer", "Buyer");
                return View();
            }
            catch (Exception e)
            {
                string GUID = Guid.NewGuid().ToString();
                _logger.LogError(e, GUID, null);
                return BadRequest("Unexpected Error " + GUID);
            }
        }
        //[HttpPost]
        //public async Task<IActionResult> VerifyCaptcha(string gRecaptchaResponse)
        //{
        //    if (string.IsNullOrWhiteSpace(gRecaptchaResponse))
        //    {
        //        ModelState.AddModelError(string.Empty, "Captcha is required.");
        //        return View("Index");
        //    }

        //    var captchaResult = await _captchaService.VerifyCaptchaAsync(gRecaptchaResponse);

        //    if (!captchaResult.Success || captchaResult.Score < 0.5)
        //    {
        //        // Show visible reCAPTCHA v2 or reject the request
        //        return View("CaptchaRequired");
        //    }

        //    // Proceed with form submission
        //    return RedirectToAction("Success");
        //}
        public IActionResult Error()
        {
            return View();
        }
        public IActionResult Users()
        {
            return View();
        }
    }
}
