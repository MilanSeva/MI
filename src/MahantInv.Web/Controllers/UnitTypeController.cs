using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static MahantInv.Infrastructure.Utility.Meta;

namespace MahantInv.Web.Controllers
{
    [Authorize(Roles = Roles.Admin + "," + Roles.User)]
    public class UnitTypeController : BaseController
    {
        public UnitTypeController(IMapper mapper) : base(mapper)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
