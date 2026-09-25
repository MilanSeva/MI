using AutoMapper;
using MahantInv.Web.Infrastructure.Dtos.Category;
using MahantInv.Web.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MahantInv.Web.Infrastructure.Utility.Meta;

namespace MahantInv.Web.Api
{
    [Authorize(Roles = Roles.Admin + "," + Roles.User + "," + Roles.ProductView)]
    public class CategoryApiController : BaseApiController
    {
        private const int MaxSuggestions = 20;
        private readonly ILogger<CategoryApiController> _logger;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryApiController(IMapper mapper, ILogger<CategoryApiController> logger, ICategoryRepository categoryRepository) : base(mapper)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
        }

        // GET api/categories/search?query=ele
        [HttpGet("categories/search")]
        public async Task<object> Search([FromQuery] string? query)
        {
            try
            {
                IEnumerable<CategoryDto> data = await _categoryRepository.SearchCategories(query, MaxSuggestions);
                return Ok(data);
            }
            catch (Exception e)
            {
                string GUID = Guid.NewGuid().ToString();
                _logger.LogError(e, GUID, null);
                return BadRequest(new { success = false, errors = new[] { "Unexpected Error " + GUID } });
            }
        }
    }
}
