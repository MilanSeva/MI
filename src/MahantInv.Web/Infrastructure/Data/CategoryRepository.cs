using MahantInv.Web.Infrastructure.Dtos.Category;
using MahantInv.Web.Infrastructure.Entities;
using MahantInv.Web.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MahantInv.Web.Infrastructure.Data
{
    public class CategoryRepository : EfRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(MIDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CategoryDto>> SearchCategories(string query, int limit)
        {
            IQueryable<Category> categories = _context.Categories;
            query = query?.Trim();
            if (!string.IsNullOrEmpty(query))
            {
                string pattern = "%" + EscapeLike(query) + "%";
                string prefix = EscapeLike(query) + "%";
                categories = categories
                    .Where(c => EF.Functions.Like(c.Name, pattern, "\\"))
                    // Prefix matches first so "Ele" ranks "Electronics" above "Home Electrical"
                    .OrderBy(c => EF.Functions.Like(c.Name, prefix, "\\") ? 0 : 1)
                    .ThenBy(c => c.Name);
            }
            else
            {
                categories = categories.OrderBy(c => c.Name);
            }

            return await categories
                .Take(limit)
                .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
                .ToListAsync();
        }

        public async Task SyncProductCategories(Product product, IEnumerable<string> categoryNames)
        {
            List<string> names = (categoryNames ?? Enumerable.Empty<string>())
                .Select(n => n?.Trim())
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Existing products must have ProductCategories.Category loaded by the caller.
            List<ProductCategory> toRemove = product.ProductCategories
                .Where(pc => !names.Contains(pc.Category.Name, StringComparer.OrdinalIgnoreCase))
                .ToList();
            foreach (ProductCategory link in toRemove)
            {
                product.ProductCategories.Remove(link);
                _context.ProductCategories.Remove(link);
            }

            List<string> toAdd = names
                .Where(n => !product.ProductCategories.Any(pc => string.Equals(pc.Category.Name, n, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            if (toAdd.Count == 0)
            {
                return;
            }

            // Categories.Name uses NOCASE collation, so this IN (...) matches case-insensitively
            List<Category> existing = await _context.Categories
                .Where(c => toAdd.Contains(c.Name))
                .ToListAsync();

            foreach (string name in toAdd)
            {
                Category category = existing.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase))
                    ?? new Category { Name = name };
                product.ProductCategories.Add(new ProductCategory { Category = category });
            }
        }

        private static string EscapeLike(string value)
        {
            return value.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
        }
    }
}
