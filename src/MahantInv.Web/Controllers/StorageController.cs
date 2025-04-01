using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static MahantInv.Infrastructure.Utility.Meta;

namespace MahantInv.Web.Controllers
{
    [Authorize(Roles = Roles.Admin + "," + Roles.User)]
    public class StorageController : BaseController
    {
        public StorageController(IMapper mapper) : base(mapper)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

    }
}
