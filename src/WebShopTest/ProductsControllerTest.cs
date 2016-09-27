//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Localization;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using WebShop.Models;
//using Xunit;

//namespace WebShop.Controllers
//{
//    public class ProductsControllerTest
//    {
//        private readonly IStringLocalizer<ProductsController> _localizer;

//        private static DbContextOptions<WebShopRepository> CreateNewContextOptions()
//        {
//            // Create a fresh service provider, and therefore a fresh 
//            // InMemory database instance.
//            var serviceProvider = new ServiceCollection()
//                .AddEntityFrameworkInMemoryDatabase()
//                .BuildServiceProvider();

//            // Create a new options instance telling the context to use an
//            // InMemory database and the new service provider.
//            var builder = new DbContextOptionsBuilder<WebShopRepository>();
//            builder.UseInMemoryDatabase()
//                   .UseInternalServiceProvider(serviceProvider);

//            return builder.Options;
//        }
//        [Fact]
//        public async Task IndexListAllProducts()
//        {
//            //Arrange
//            // All contexts that share the same service provider will share the same InMemory database
//            var options = CreateNewContextOptions();
            

//            // Insert seed data into the database using one instance of the context
//            using (var context = new WebShopRepository(options))
//            {
//                context.ProductTranslation.Add(new Product { ProductName = "Dragon" });
//                context.ProductTranslation.Add(new Product { ProductName = "Sexy hat" });
//                context.ProductTranslation.Add(new Product { ProductName = "Crohet earrings" });

//                context.SaveChanges();
//            }

//            // Use a clean instance of the context to run the test
//            using (var context = new WebShopRepository(options))
//            {
//                var service = new ProductsController(context, _localizer);

//                //Act
//                var result = await service.Index();

//                //Assert
//                var viewResult = Assert.IsType<ViewResult>(result);
//                var model = Assert.IsAssignableFrom<IEnumerable<WebShop.Models.Product>>(
//                    viewResult.ViewData.Model);
//                Assert.Equal(3, model.Count());
//                Assert.Equal("Dragon", model.ElementAt(0).ProductName);
//            }
//        }

//        [Fact]
//        public async Task TestSearchProductEngine()
//        {
//            var options = CreateNewContextOptions();

//            using (var context = new WebShopRepository(options))
//            {
//                context.ProductTranslation.Add(new Product { ProductName = "Dragon" });
//                context.ProductTranslation.Add(new Product { ProductName = "Sexy hat" });
//                context.ProductTranslation.Add(new Product { ProductName = "Crohet earrings" });

//                context.SaveChanges();
//            }

//            using (var context = new WebShopRepository(options))
//            {
//                var productController = new ProductsController(context, _localizer);
//                //productController.ModelState.AddModelError("test", "test");

//                //Act
//                var resultFromTheCalledActionMethod = await productController.SearchForProduct("Dragon");

//                var viewResult = Assert.IsType<ViewResult>(resultFromTheCalledActionMethod);
//                var model = Assert.IsAssignableFrom<IEnumerable<WebShop.Models.Product>>(
//                    viewResult.ViewData.Model);
//                foreach (var item in model)
//                {
//                    Assert.Contains("dragon", item.ProductName);

//                }

//                Assert.Equal(1, model.Count());
              

//            }


//        }


//        }
//}