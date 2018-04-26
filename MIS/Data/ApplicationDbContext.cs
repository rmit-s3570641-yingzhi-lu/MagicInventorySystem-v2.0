using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MIS.Models;

namespace MIS.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //composite key
            builder.Entity<StoreInventory>().HasKey(x => new { x.StoreID, x.ProductID });
        }

        public DbSet<MIS.Models.Product> Product { get; set; }
        public DbSet<MIS.Models.OwnerInventory> OwnerInventory { get; set; }
        public DbSet<MIS.Models.Store> Store { get; set; }
        public DbSet<MIS.Models.StoreInventory> StoreInventory { get; set; }
        public DbSet<MIS.Models.StockRequest> StockRequest { get; set; }
    }
}
