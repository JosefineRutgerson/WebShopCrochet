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
            ProductCategory prodCat = new ProductCategory { ProductCategoryId = 1, ProductCategoryName = "amigu", };

            using (var context = new WebShopRepository(options))
            {
                
                context.ProductCategories.Add(prodCat);
                context.Products.Add(new Product { ProductId = 5, Price = 10, ImageName = "drake.jpg", ProductCategoryId = 1, ProductCategory = prodCat });
                context.Products.Add(new Product { ProductId = 2, Price = 20, ImageName = "dra.jpg", ProductCategoryId = 1, ProductCategory = prodCat });
                context.Products.Add(new Product { ProductId = 3, Price = 30, ImageName = "drake.jpg", ProductCategoryId = 1, ProductCategory = prodCat });
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Dragon", Language = "en", ProductId = 5, ProductDescription = "dragons" });
                context.ProductTranslations.Add(new ProductTranslation { ProductName = "Drake", Language = "sv", ProductId = 5, ProductDescription = "drakar" });
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
            SeedData(options);

            // Use a clean instance of the context to run the test
            using (var context = new WebShopRepository(options))
            {
                var service = new ProductsController(context, _localizer);

                //Act
                var result = await service.Index(exchangeRate);

                //Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<WebShop.Models.ProductViewModel>>(
                    viewResult.ViewData.Model);
                Assert.Equal(3, model.Count());
                Assert.Equal("Dragon", model.ElementAt(0).ProductName);
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
            //ProductCategory prodCat = new ProductCategory { ProductCategoryId = 1, ProductCategoryName = "amigu", };
            //using (var context = new WebShopRepository(options))
            //{
            //    context.ProductCategories.Add(prodCat);
            //    context.Products.Add(new Product { ProductId = 1, Price = 10, ImageName = "drake.jpg", ProductCategoryId = 1, ProductCategory = prodCat });
            //    context.Products.Add(new Product { ProductId = 2, Price = 20, ImageName = "dra.jpg", ProductCategoryId = 1, ProductCategory = prodCat });
            //    context.Products.Add(new Product { ProductId = 3, Price = 30, ImageName = "drake.jpg", ProductCategoryId = 1, ProductCategory = prodCat });
            //    context.ProductTranslations.Add(new ProductTranslation { ProductName = "Dragon", Language = "en", ProductId = 1, ProductDescription = "dragons" });
            //    context.ProductTranslations.Add(new ProductTranslation { ProductName = "Drake", Language = "sv", ProductId = 1, ProductDescription = "drakar" });
            //    context.ProductTranslations.Add(new ProductTranslation { ProductName = "Hat", Language = "en", ProductId = 2, ProductDescription = "hats" });
            //    context.ProductTranslations.Add(new ProductTranslation { ProductName = "Hatt", Language = "sv", ProductId = 2, ProductDescription = "Hattar" });
            //    context.ProductTranslations.Add(new ProductTranslation { ProductName = "Cake", Language = "en", ProductId = 3, ProductDescription = "cakes" });
            //    context.ProductTranslations.Add(new ProductTranslation { ProductName = "Tårta", Language = "sv", ProductId = 3, ProductDescription = "Tårtor" });


            //    context.SaveChanges();
            //}


            // Insert seed data into the database using one instance of the context
            var options = CreateNewContextOptions();
            SeedData(options);

            // Use a clean instance of the context to run the test
            using (var context = new WebShopRepository(options))
            {
                var service = new ProductsController(context, _localizer);

                //Act
                var result = await service.Details(5);

                //Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<WebShop.Models.ProductViewModel>(
                    viewResult.ViewData.Model);
                
                Assert.Equal(5, model.ProductId);
                Assert.Equal(10, model.Price);
                Assert.Equal("Dragon", model.ProductName);
                //Assert.Equal("Dragon", model.ElementAt(0).Translations.SingleOrDefault().ProductName);
            }
        }


        [Fact]
        public async Task TestForCreateMethod()
        {
            //Arrange
            var options = CreateNewContextOptions();

            //Seed data
            SeedData(options);
            //Use a clean instance of the context
            using (var context = new WebShopRepository(options) )
            {
                var service = new ProductsController(context, _localizer);
                ProductCategory productCategory = new ProductCategory{ ProductCategoryId = 4, ProductCategoryName = "Hat"};
                ProductTranslation pTrans = new ProductTranslation {ProductName = "Cat", Language = "en", ProductDescription="Cats" };
                ProductTranslation pTrans2 = new ProductTranslation { ProductName = "Katt", Language = "sv", ProductDescription = "Katter" };
                Product prool = new Product { ProductCategoryId = 4, ProductCategory = productCategory, ImageName = "cat.jpg", Price = 40  };
                AllProductData APD = new AllProductData();
                
                APD.Price = prool.Price;
                APD.ImageName = prool.ImageName;
                APD.ProductCategory = prool.ProductCategory;
                APD.ProductCategoryId = prool.ProductCategoryId;
                APD.ProductName = pTrans.ProductName;
                APD.ProductDescription = pTrans.ProductDescription;
                APD.Language = pTrans.Language;
                APD.ProductNameSV = pTrans2.ProductName;
                APD.ProductDescriptionSV = pTrans2.ProductDescription;
                APD.LanguageSV = pTrans2.Language;

                //Act = kalla på methoden
                var result = await service.Create(APD);
                //Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<WebShop.Models.AllProductData>(viewResult.ViewData.Model);

                
                Assert.Equal(4, model.ProductCategoryId);
                Assert.Equal("Hat", model.ProductCategory.ProductCategoryName);
                Assert.Equal(40, model.Price);
                Assert.Equal("Cat", model.ProductName);
                Assert.Equal("Katt", model.ProductNameSV);


            }

        }

        [Fact]
        public async Task TestForEditMethod()
        {
            var options = CreateNewContextOptions();
            SeedData(options);
            //Use clen instance of context
            using (var context = new WebShopRepository(options))
            {
                var services = new ProductsController(context, _localizer);

                AllProductData APD = new AllProductData();
                APD.ProductId = 2;
                APD.ProductName = "Dog";
                APD.ProductDescription = "Dogs";
                APD.Language = "en";
                APD.ProductNameSV = "Hund";
                APD.ProductDescriptionSV = "Hundar";
                APD.LanguageSV = "sv";
                APD.ImageName = "hung.jpg";
                APD.Price = 21;
                APD.ProductCategoryId = 1;

                var result = await services.Edit(APD, 2);
                var result2 = await services.Index(exchangeRate);

                //var viewResult = Assert.IsType<RedirectToActionResult>(result);
                var viewR2 = Assert.IsType<ViewResult>(result2);
                var model = Assert.IsAssignableFrom<IEnumerable<WebShop.Models.ProductViewModel>>(viewR2.ViewData.Model);

                Assert.Equal(3, model.Count());
                Assert.Equal("Dog", model.ElementAt(1).ProductName);
                //Assert.Equal(2, model.ProductId);
                //Assert.Equal("Dog", model.ProductName);
                //Assert.Equal(21, model.Price);
                
            }
        }

        [Fact]
        public async Task TestDeleteMethod()
        {
            var options = CreateNewContextOptions();
            SeedData(options);
            using (var context = new WebShopRepository(options))
            {
                var service = new ProductsController(context, _localizer);
                var result = service.Delete(2);

                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<WebShop.Models.ProductViewModel>(viewResult.ViewData.Model);

                //Assert.Equal(2, model.ProductId);
                Assert.Equal("Hat", model.ProductName);
            }
        }

        [Fact]
        public async Task TestDeleteConfirmedMethod()
        {
            var options = CreateNewContextOptions();
            //seed data
            SeedData(options);

            //Use clean instance
            using (var context = new WebShopRepository(options))
            {
                var service = new ProductsController(context, _localizer);

                var result = await service.DeleteConfirmed(2);
                var result2 = await service.Index(exchangeRate);

                var viewResult = Assert.IsType<ViewResult>(result2);
                var model = Assert.IsAssignableFrom<IEnumerable<WebShop.Models.ProductViewModel>>(viewResult.ViewData.Model);

                Assert.Equal(2, model.Count()); 
                              


            }
        }
    }
}