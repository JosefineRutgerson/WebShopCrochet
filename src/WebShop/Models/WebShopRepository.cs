using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebShop.Models
{
    public class WebShopRepository : DbContext
    {
        public WebShopRepository(DbContextOptions<WebShopRepository> options) :
            base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductTranslation> ProductTranslations { get; set; }
        // public DbSet<ProductViewModel> ProductViewModels { get; set; }
        public DbSet<Cart> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductTranslation>()
            .HasKey(c => new { c.ProductId, c.Language });
        }


    }
}
