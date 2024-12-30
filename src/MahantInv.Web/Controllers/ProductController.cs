using AutoMapper;
using MahantInv.Infrastructure.Data;
using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Interfaces;
using MahantInv.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MahantInv.Web.Controllers
{
    public class ProductController : BaseController
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IAsyncRepository<Storage> _storageRepository;
        private readonly IAsyncRepository<UnitType> _unitTypeRepository;
        private readonly IAsyncRepository<ProductUsage> _productUsageRepository;
        private readonly MIDbContext _context;
        public ProductController(IMapper mapper, MIDbContext context, ILogger<ProductController> logger, IAsyncRepository<ProductUsage> productUsageRepository, IAsyncRepository<Storage> storageRepository, IAsyncRepository<UnitType> unitTypeRepository) : base(mapper)
        {
            _logger = logger;
            _storageRepository = storageRepository;
            _unitTypeRepository = unitTypeRepository;
            _productUsageRepository = productUsageRepository;
            _context = context;
        }

        public async Task<IActionResult> Index([FromServices] IBuyersRepository _buyersRepository)
        {
            try
            {
                ViewBag.Storages = await _storageRepository.ListAllAsync();
                ViewBag.UnitTypes = await _unitTypeRepository.ListAllAsync();
                ViewBag.OrderBulkNames = await _context.Products.Where(p => p.OrderBulkName != null && p.OrderBulkName != "")
                            .Select(p => p.OrderBulkName).Distinct().ToListAsync();
                ViewBag.Buyers = new SelectList((await _productUsageRepository.ListAllAsync()), "Buyer", "Buyer");
                return View();
            }
            catch (Exception e)
            {
                string GUID = Guid.NewGuid().ToString();
                _logger.LogError(e, GUID, null);
                return BadRequest("Unexpected Error " + GUID);
            }
        }
    }
}
