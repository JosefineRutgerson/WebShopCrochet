using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebShop.Models;
using WebShop.Services;
using Xunit;

namespace WebShop.Controllers
{
    public class ProductsControllerTest
    {
        private readonly IStringLocalizer<ProductsController> _localizer;
        FixerIo exchangeRate = new FixerIo();
        

        private static DbContextOptions<WebShopRepository> CreateNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<WebShopRepository>();
            builder.UseInMemoryDatabase()
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }

        public void SeedData(DbContextOptions<WebShopRepository> options)
        {
            

            using (var context = new WebShopRepository(options))
            {
                context.Products.Add(new Product { ProductId = 1, Price = 10 });
                context.Products.Add(new Product { ProductId = 2, Price = 20 });
                context.Products.Add(new Product { ProductId = 3, Price = 30 });
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Dragon", Language = "en", ProductId = 1, ProductDescription = "dragons" });
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Drake", Language = "sv", ProductId = 1, ProductDescription = "drakar" });
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Hat", Language = "en", ProductId = 2, ProductDescription = "hats" });
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Hatt", Language = "sv", ProductId = 2, ProductDescription = "Hattar" });
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Cake", Language = "en", ProductId = 3, ProductDescription = "cakes" });
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Tårta", Language = "sv", ProductId = 3, ProductDescription = "Tårtor" });


                context.SaveChanges();
            }

        }

        [Fact]
        public async Task IndexListAllProducts()
        {
            //Arrange
            // All contexts that share the same service provider will share the same InMemory database
            var options = CreateNewContextOptions();


            // Insert seed data into the database using one instance of the context
            using (var context = new WebShopRepository(options))
            {
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Dragon" });
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Sexy hat" });
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Crohet earrings" });

                context.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            using (var context = new WebShopRepository(options))
            {
                var service = new ProductsController(context, _localizer);

                //Act
                var result = await service.Index(exchangeRate);

                //Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<WebShop.Models.Product>>(
                    viewResult.ViewData.Model);
                Assert.Equal(3, model.Count());
                Assert.Equal("Dragon", model.ElementAt(0).Translations.SingleOrDefault().ProductName);
            }
        }

        [Fact]
        public async Task TestSearchProductEngine()
        {
            var options = CreateNewContextOptions();

            using (var context = new WebShopRepository(options))
            {
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Dragon" });
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Sexy hat" });
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Crohet earrings" });

                context.SaveChanges();
            }

            using (var context = new WebShopRepository(options))
            {
                var productController = new ProductsController(context, _localizer);
                //productController.ModelState.AddModelError("test", "test");

                //Act
                var resultFromTheCalledActionMethod = await productController.SearchForProduct("Dragon");

                var viewResult = Assert.IsType<ViewResult>(resultFromTheCalledActionMethod);
                var model = Assert.IsAssignableFrom<IEnumerable<WebShop.Models.Product>>(
                    viewResult.ViewData.Model);
                foreach (var item in model)
                {
                    Assert.Contains("dragon", item.Translations.SingleOrDefault().ProductName);

                }

                Assert.Equal(1, model.Count());
                
            }

        }


        [Fact]
        public async Task DetailsListProduct()
        {
            //Arrange
            // All contexts that share the same service provider will share the same InMemory database
            var options = CreateNewContextOptions();

            using (var context = new WebShopRepository(options))
            {
                context.Products.Add(new Product { ProductId = 1, Price = 10 });
                context.Products.Add(new Product { ProductId = 2, Price = 20 });
                context.Products.Add(new Product { ProductId = 3, Price = 30 });
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Dragon", Language = "en", ProductId = 1, ProductDescription = "dragons" });
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Drake", Language = "sv", ProductId = 1, ProductDescription = "drakar" });
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Hat", Language = "en", ProductId = 2, ProductDescription = "hats" });
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Hatt", Language = "sv", ProductId = 2, ProductDescription = "Hattar" });
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Cake", Language = "en", ProductId = 3, ProductDescription = "cakes" });
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Tårta", Language = "sv", ProductId = 3, ProductDescription = "Tårtor" });


                context.SaveChanges();
            }


            // Insert seed data into the database using one instance of the context
            //SeedData(options);

            // Use a clean instance of the context to run the test
            using (var context = new WebShopRepository(options))
            {
                var service = new ProductsController(context, _localizer);

                //Act
                var result = await service.Details(1);

                //Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<WebShop.Models.ProductViewModel>(
                    viewResult.ViewData.Model);
                
                Assert.Equal(1, model.ProductId);
                Assert.Equal(10, model.Price);
                //Assert.Equal("Dragon", model.ElementAt(0).Translations.SingleOrDefault().ProductName);
            }
        }


    }
}