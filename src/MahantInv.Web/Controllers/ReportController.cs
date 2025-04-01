using MahantInv.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using static MahantInv.Infrastructure.Utility.Meta;

namespace MahantInv.Web.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class ReportController : Controller
    {
        private readonly IAdHocRepo _adHocRepo;

        public ReportController(IAdHocRepo adHocService)
        {
            _adHocRepo = adHocService;
        }
        public async Task<IActionResult> fa9f3be62c748689d5b041dadf4ccf9()
        {
            ViewBag.Tables = await _adHocRepo.GetSchemaAsync();
            ViewBag.Query = "";
            return View(null);
        }
        private bool IsSystemUser()
        {
            return User.Identity.Name.Equals("msystem", System.StringComparison.OrdinalIgnoreCase) || User.Identity.Name.Equals("system", System.StringComparison.OrdinalIgnoreCase);
        }
        [HttpPost]
        public async Task<IActionResult> fa9f3be62c748689d5b041dadf4ccf9([FromBody] string query)
        {
            try
            {
                if (IsSystemUser())
                {
                    ViewBag.Tables = await _adHocRepo.GetSchemaAsync();
                    ViewBag.Query = query;
                    var result = await _adHocRepo.QueryObjectAsync(query);
                    JsonSerializerOptions o = new()
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    return Json(JsonSerializer.Serialize(result, o));
                }
                return Json(JsonSerializer.Serialize("[]"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
