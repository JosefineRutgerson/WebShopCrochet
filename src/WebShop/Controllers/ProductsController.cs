using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebShop.Models;
using Microsoft.Extensions.Localization;
using WebShop.Helper_Class;
using WebShop.Services;
using System.Globalization;

namespace WebShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IStringLocalizer<ProductsController> _localizer;
        private FixerIo exchangeRate;
        private readonly WebShopRepository _context;
        private SqlHelperClass sqlHelper;
        private ProductTranslationsController productTranslationcontroller;


        public ProductsController(WebShopRepository context, IStringLocalizer<ProductsController> localizer )
        {
            _localizer = localizer;
            _context = context;
            
            sqlHelper = new SqlHelperClass(_context);
            
        }

        [HttpGet]
        public string GetTranslatedString(string translateWord)
        {

            return _localizer[translateWord];
        }

        
        // GET: Products
        public async Task<IActionResult> Index(FixerIo rate)
        {
            exchangeRate = rate;
            var productList = sqlHelper.PopulateProductViewModel().ToList();

            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName != "sv")
            {
                for (int i = 0; i < productList.Count(); i++)
                {
                    productList[i].Price = exchangeRate.ConvertPrice("SEK", "USD", productList[i].Price);
                }               
            }

            //productList.ToAsyncEnumerable();
            return View(await productList.ToAsyncEnumerable().ToList());

        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            var productViewModels = sqlHelper.PopulateProductViewModel();
            
            //var product = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == id);
            foreach (var item in productViewModels)
            {
                if (item.ProductId == id)
                    return View(item);

            }
            //if (productViewModel == null)
            //{
            //    return NotFound();
            //}

            return View("Index");
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["ProductCategoryId"] = new SelectList(_context.ProductCategories, "ProductCategoryId", "ProductCategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create([Bind("ProductId, ProductName, ProductNameSV, ProductDescription, ProductDescriptionSV, Price, ProductCategoryId, ImageName")] AllProductData productData)
        {

            Product prool = new Product();
            prool.Price = productData.Price;
            _context.Add(prool);
            _context.SaveChanges();

            if (ModelState.IsValid)
            {
                sqlHelper.InsertProduct(productData);
                return RedirectToAction("Index");
                //_context.Add(product);
                //await _context.SaveChangesAsync();
                //return RedirectToAction("Create", "ProductTranslations", new { id = product.ProductId });

            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }
            ViewData["ProductCategoryId"] = new SelectList(_context.ProductCategories, "ProductCategoryId", "ProductCategoryName", productData.ProductCategoryId);
            return View(productData);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int theID = id ?? default(int);
            AllProductData allProductData = sqlHelper.PopulateAllProductData(theID);
            //var product = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == id);
            if (allProductData == null)
            {
                return NotFound();
            }
            ViewData["ProductCategoryId"] = new SelectList(_context.ProductCategories, "ProductCategoryId", "ProductCategoryName", allProductData.ProductCategoryId);
            return View(allProductData);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ProductId, ProductName, ProductNameSV, ProductDescription, ProductDescriptionSV, Price, ProductCategoryId, ImageName")] AllProductData productData, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            productData.ProductId = id ?? default(int);
            if (ModelState.IsValid)
            {
                try
                {
                    sqlHelper.UpdateProduct(productData);
                    //_context.Update(productData);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(productData.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["ProductCategoryId"] = new SelectList(_context.ProductCategories, "ProductCategoryId", "ProductCategoryName", productData.ProductCategoryId);
            return View(productData);
        }


       // Filters products
        public async Task<IActionResult> SearchForProduct(string searchInput)
        {

            //AllProductData searchProduct = new AllProductData();
            var products = sqlHelper.PopulateProductViewModel().ToList();
            //var productviewList = products.Where(pro => pro.ProductName.Contains(searchInput)).ToList();
            List<ProductViewModel> productviemmodel = new List<ProductViewModel>();

            foreach (var item in products)
            {
                productviemmodel = products.Where(pro => pro.ProductName.Contains(searchInput)).ToList();

            }

            //var webShopRepository = _context.Products.Where(p => p.ProductId.Contains(searchInput));
            return View("Index", productviemmodel);
        }



        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productViewModels = sqlHelper.PopulateProductViewModel();

            foreach (var item in productViewModels)
            {
                if (item.ProductId == id)
                    return View(item);

            }

            //var product = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == id);
            

            return View("Index");
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            sqlHelper.DeleteProduct(id);
            //var product = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == id);
            //_context.Products.Remove(product);
            //await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
