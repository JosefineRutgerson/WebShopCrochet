using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebShop.Models;
using Microsoft.AspNetCore.Http;
using WebShop.Helper_Class;
using System.Globalization;
using WebShop.Services;

namespace WebShop.Controllers
{
    public class ShoppingCartsController : Controller
    {
        private readonly WebShopRepository _context;
        FixerIo exchangeRate;
        //private ShoppingCart _shoppingCart;
        private SqlHelperClass sqlHelper;

        public ShoppingCartsController(WebShopRepository context)
        {
            _context = context;
            sqlHelper = new SqlHelperClass(_context);
        }


        public IActionResult Index(FixerIo rate)
        {
            exchangeRate = rate;
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
                item.Product.Price = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "sv" ? item.Product.Price : exchangeRate.ConvertPrice("SEK", "USD", item.Product.Price); 
                viewModel.ImageName = item.Product.ImageName;
                

            }
            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName != "sv")
            {
                viewModel.CartTotal = exchangeRate.ConvertPrice("SEK", "USD", viewModel.CartTotal);
            }
         
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

        
        public ActionResult RemoveFromCart(int id)
        {
            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(this.HttpContext, _context);

            // Get the name of the album to display confirmation
            //string productName = _context.Carts
            //    .Single(item => item.Product.ProductId == id).Product.Translations.SingleOrDefault().ProductName;

            // Remove from cart
            int itemCount = cart.RemoveFromCart(id);

            // Display the confirmation message
            //var results = new ShoppingCartRemoveViewModel
            //{
            //    Message = System.Net.WebUtility.HtmlEncode(productName) +
            //        " has been removed from your shopping cart.",
            //    CartTotal = cart.GetTotal(),
            //    CartCount = cart.GetCount(),
            //    ItemCount = itemCount,
            //    DeleteId = id
            //};
            //return Json(results);
            return RedirectToAction("Index");
        }

        //public ActionResult Delete()
        //{

        //}


    }
}