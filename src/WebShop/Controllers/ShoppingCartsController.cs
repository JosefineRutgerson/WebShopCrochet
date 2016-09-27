using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebShop.Models;
using Microsoft.AspNetCore.Http;
using WebShop.Helper_Class;
using System.Globalization;

namespace WebShop.Controllers
{
    public class ShoppingCartsController : Controller
    {
        private readonly WebShopRepository _context;
        //private ShoppingCart _shoppingCart;
        private SqlHelperClass sqlHelper;

        public ShoppingCartsController(WebShopRepository context)
        {
            _context = context;
            sqlHelper = new SqlHelperClass(_context);
        }


        public IActionResult Index()
        {

            ShoppingCart _shoppingCart = ShoppingCart.GetCart(this.HttpContext, _context);
            //var shoppingcartList = _shoppingCart.GetCartItems();


            var viewModel = new ShoppingCartViewModel
            {
                CartItems = _shoppingCart.GetCartItems(),
                CartTotal = _shoppingCart.GetTotal(),
                               
            };
            foreach (var item in viewModel.CartItems)
            {

                //item.Translation.ProductName
                item.Product.Translations = item.Product.Translations.Where(x => x.Language == CultureInfo.CurrentCulture.TwoLetterISOLanguageName).ToList();
                viewModel.ProductName = item.Product.Translations.SingleOrDefault().ProductName;
                viewModel.Price = item.Product.Price;
                viewModel.ImageName = item.Product.ImageName;

            }
            //int id = viewModel.CartItems.Select(x => x.ProductId).Where()
            //viewModel.ProductName = sqlHelper.GetProAndTransl(viewModel.CartItems.Where(x => x.ProductId)
            //foreach (var item in shoppingcartList)
            //{
            //    if (item.CartId == item.Id.ToString())
            //    {
            //        return View(item);
            //    }
            //}

            //for (int i = 0; i < viewModel.CartItems.Count(); i++)
            //{
            //    viewModel.CartItems[i].ProductData = sqlHelper.PopulateAllProductData(viewModel.CartItems[i].ProductId);
            //}

            return View(viewModel);
            //return View(shoppingcartList);

        }

        public ActionResult AddToCart(int id)
        {
            // Retrieve the album from the database
            var addedProduct = _context.Products
                .Single(Product => Product.ProductId == id);

            // Add it to the shopping cart
            var cart = ShoppingCart.GetCart(this.HttpContext, _context);

            cart.AddToCart(addedProduct);

            // Go back to the main store page for more shopping
            return RedirectToAction("Index");
        }

        public ActionResult Delete()
        {

        }


    }
}