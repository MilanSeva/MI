using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace MahantInv.Web.Controllers
{
    public class DownloadController : BaseController
    {
        private readonly ILogger<DownloadController> _logger;

        public DownloadController(ILogger<DownloadController> logger, IMapper mapper) : base(mapper)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                string guid = Guid.NewGuid().ToString();
                _logger.LogError(e, guid, null);
                return BadRequest("Unexpected Error " + guid);
            }
        }
    }
}
