using Microsoft.EntityFrameworkCore;
using Tutorial9.Model;

namespace Tutorial9.Data
{
    public class WarehouseDbContext : DbContext
    {
        public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product_Warehouse> Product_Warehouses { get; set; }
    }
}