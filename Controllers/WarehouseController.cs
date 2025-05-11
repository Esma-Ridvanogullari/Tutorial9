using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Tutorial9.Data;
using Tutorial9.Model;

namespace Tutorial9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly WarehouseDbContext _context;

        public WarehouseController(WarehouseDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToWarehouse([FromBody] ProductWarehouseRequest request)
        {
            var product = await _context.Products.FindAsync(request.ProductId);
            var warehouse = await _context.Warehouses.FindAsync(request.WarehouseId);

            if (product == null || warehouse == null)
                return NotFound("Product or Warehouse not found.");

            if (request.Amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.ProductId == request.ProductId && o.Amount == request.Amount && o.CreatedAt < request.CreatedAt && o.FullfilledAt == null);

            if (order == null)
                return BadRequest("No matching order found.");

            order.FullfilledAt = DateTime.Now;
            _context.Orders.Update(order);

            var productWarehouse = new Product_Warehouse
            {
                ProductId = request.ProductId,
                WarehouseId = request.WarehouseId,
                Price = product.Price * request.Amount,
                Amount = request.Amount,
                CreatedAt = DateTime.Now
            };

            _context.Product_Warehouses.Add(productWarehouse);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(AddProductToWarehouse), new { id = productWarehouse.Id }, productWarehouse);
        }
        [HttpGet("{warehouseId}/products")]
        public async Task<IActionResult> GetProductsInWarehouse(int warehouseId)
        {
            var productsInWarehouse = await _context.Product_Warehouses
                .Where(pw => pw.WarehouseId == warehouseId)
                .Select(pw => new
                {
                    ProductId = pw.ProductId,
                    ProductName = pw.Product.Name,
                    Price = pw.Price,
                    Amount = pw.Amount
                }).ToListAsync();

            return Ok(productsInWarehouse);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddProductToWarehouseUsingSP([FromBody] ProductWarehouseRequest request)
        {
            var productIdParam = new SqlParameter("@ProductId", request.ProductId);
            var warehouseIdParam = new SqlParameter("@WarehouseId", request.WarehouseId);
            var amountParam = new SqlParameter("@Amount", request.Amount);
            var createdAtParam = new SqlParameter("@CreatedAt", request.CreatedAt);

            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC AddProductToWarehouseSP @ProductId, @WarehouseId, @Amount, @CreatedAt", 
                    productIdParam, warehouseIdParam, amountParam, createdAtParam);
                return Ok("Product added to warehouse using stored procedure.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


    }
}
