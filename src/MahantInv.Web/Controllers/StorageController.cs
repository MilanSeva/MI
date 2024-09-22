using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace MahantInv.Web.Controllers
{
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
