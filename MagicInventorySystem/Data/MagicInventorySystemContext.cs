using Microsoft.EntityFrameworkCore;
using MagicInventorySystem.Models;

namespace MagicInventorySystem.Models
{
    public class MagicInventorySystemContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<OwnerInventory> OwnerInventory { get; set; }
        public DbSet<StockRequest> StockRequests { get; set; }
        public DbSet<StoreInventory> StoreInventory { get; set; }

        public MagicInventorySystemContext(DbContextOptions<MagicInventorySystemContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreInventory>().HasKey(x => new { x.StoreID, x.ProductID });
        }

    }
}
