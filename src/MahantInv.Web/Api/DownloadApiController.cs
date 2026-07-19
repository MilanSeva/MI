using AutoMapper;
using MahantInv.Infrastructure.Data;
using MahantInv.Infrastructure.Interfaces;
using MahantInv.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml.Style;

namespace MahantInv.Web.Api
{
    [Authorize(Roles = "Admin")]
    public class DownloadApiController : BaseApiController
    {
        private readonly ILogger<DownloadApiController> _logger;
        private readonly IProductsRepository _productRepository;
        private readonly IOrdersRepository _orderRepository;
        private readonly IProductUsageRepository _productUsageRepository;
        private readonly MIDbContext _context;

        public DownloadApiController(
            MIDbContext context,
            IMapper mapper,
            ILogger<DownloadApiController> logger,
            IProductsRepository productRepository,
            IOrdersRepository orderRepository,
            IProductUsageRepository productUsageRepository)
            : base(mapper)
        {
            _logger = logger;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _productUsageRepository = productUsageRepository;
            _context = context;

            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        [HttpGet("download/products")]
        public async Task<IActionResult> DownloadProducts()
        {
            try
            {
                IEnumerable<ProductVM> products = await _productRepository.GetProducts();

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Products");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Name";
                    worksheet.Cells[1, 2].Value = "Gujarati Name";
                    worksheet.Cells[1, 3].Value = "Company";
                    worksheet.Cells[1, 4].Value = "Description";
                    worksheet.Cells[1, 5].Value = "Size & Unit";
                    worksheet.Cells[1, 6].Value = "Order Bulk Name";
                    worksheet.Cells[1, 7].Value = "Order Bulk Quantity";
                    worksheet.Cells[1, 8].Value = "Current Stock";
                    worksheet.Cells[1, 9].Value = "Reorder Level";
                    worksheet.Cells[1, 10].Value = "Is Disposable?";
                    worksheet.Cells[1, 11].Value = "Storage";

                    // Style header row
                    var headerRange = worksheet.Cells[1, 1, 1, 11];
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    // Add data
                    int row = 2;
                    foreach (var product in products)
                    {
                        worksheet.Cells[row, 1].Value = product.Name;
                        worksheet.Cells[row, 2].Value = product.GujaratiName ?? string.Empty;
                        worksheet.Cells[row, 3].Value = product.Company ?? string.Empty;
                        worksheet.Cells[row, 4].Value = product.Description ?? string.Empty;
                        worksheet.Cells[row, 5].Value = product.SizeUnitTypeCode ?? string.Empty;
                        worksheet.Cells[row, 6].Value = product.OrderBulkName ?? string.Empty;
                        worksheet.Cells[row, 7].Value = product.OrderBulkQuantity ?? 0;
                        worksheet.Cells[row, 8].Value = product.CurrentStock;
                        worksheet.Cells[row, 9].Value = product.ReorderLevel ?? 0;
                        worksheet.Cells[row, 10].Value = product.Disposable;
                        worksheet.Cells[row, 11].Value = product.Storage ?? string.Empty;
                        row++;
                    }

                    // Auto fit columns
                    worksheet.Cells.AutoFitColumns();

                    var fileContents = package.GetAsByteArray();
                    return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Products.xlsx");
                }
            }
            catch (Exception e)
            {
                string guid = Guid.NewGuid().ToString();
                _logger.LogError(e, guid, null);
                return BadRequest(new { success = false, errors = new[] { "Unexpected Error " + guid } });
            }
        }

        [HttpGet("download/purchase")]
        public async Task<IActionResult> DownloadPurchase()
        {
            try
            {
                var orders = await _orderRepository.GetOrders();

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Purchase");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Product";
                    worksheet.Cells[1, 2].Value = "Order Date";
                    worksheet.Cells[1, 3].Value = "Ordered Quantity";
                    worksheet.Cells[1, 4].Value = "Received Quantity";
                    worksheet.Cells[1, 5].Value = "Payment Status";
                    worksheet.Cells[1, 6].Value = "Seller";
                    worksheet.Cells[1, 7].Value = "Order Status";
                    worksheet.Cells[1, 8].Value = "Received Date";
                    worksheet.Cells[1, 8].Value = "Remark";
                    worksheet.Cells[1, 10].Value = "Order Bulk Name";
                    worksheet.Cells[1, 11].Value = "Last Modified By";

                    // Style header row
                    var headerRange = worksheet.Cells[1, 1, 1, 12];
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    // Add data
                    int row = 2;
                    foreach (var order in orders)
                    {
                        worksheet.Cells[row, 1].Value = order.Product ?? string.Empty;
                        worksheet.Cells[row, 2].Value = order.OrderDate ?? string.Empty;
                        worksheet.Cells[row, 3].Value = order.Quantity ?? 0;
                        worksheet.Cells[row, 4].Value = order.ReceivedQuantity ?? 0;
                        worksheet.Cells[row, 5].Value = order.PaymentStatus ?? string.Empty;
                        worksheet.Cells[row, 6].Value = order.Seller ?? string.Empty;
                        worksheet.Cells[row, 7].Value = order.Status ?? string.Empty;
                        worksheet.Cells[row, 8].Value = order.ReceivedDate ?? string.Empty;
                        worksheet.Cells[row, 9].Value = order.Remark ?? string.Empty;
                        worksheet.Cells[row, 10].Value = order.OrderBulkName ?? string.Empty;
                        worksheet.Cells[row, 11].Value = order.LastModifiedBy ?? string.Empty;
                        row++;
                    }

                    // Auto fit columns
                    worksheet.Cells.AutoFitColumns();

                    var fileContents = package.GetAsByteArray();
                    return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Purchase.xlsx");
                }
            }
            catch (Exception e)
            {
                string guid = Guid.NewGuid().ToString();
                _logger.LogError(e, guid, null);
                return BadRequest(new { success = false, errors = new[] { "Unexpected Error " + guid } });
            }
        }

        [HttpGet("download/sell")]
        public async Task<IActionResult> DownloadSell()
        {
            try
            {
                var productUsages = await _productUsageRepository.GetProductUsages();

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sell");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Product";
                    worksheet.Cells[1, 2].Value = "Quantity";
                    worksheet.Cells[1, 3].Value = "Buyer";
                    worksheet.Cells[1, 4].Value = "Usage Date";

                    // Style header row
                    var headerRange = worksheet.Cells[1, 1, 1, 4];
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    // Add data
                    int row = 2;
                    foreach (var usage in productUsages)
                    {
                        worksheet.Cells[row, 1].Value = usage.ProductName ?? string.Empty;
                        worksheet.Cells[row, 2].Value = usage.Quantity ?? 0;
                        worksheet.Cells[row, 3].Value = usage.Buyer ?? string.Empty;
                        worksheet.Cells[row, 4].Value = usage.UsageDateFormat ?? string.Empty;
                        row++;
                    }

                    // Auto fit columns
                    worksheet.Cells.AutoFitColumns();

                    var fileContents = package.GetAsByteArray();
                    return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Sell.xlsx");
                }
            }
            catch (Exception e)
            {
                string guid = Guid.NewGuid().ToString();
                _logger.LogError(e, guid, null);
                return BadRequest(new { success = false, errors = new[] { "Unexpected Error " + guid } });
            }
        }
    }
}
