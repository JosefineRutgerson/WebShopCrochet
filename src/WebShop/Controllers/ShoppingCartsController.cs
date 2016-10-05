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
using Newtonsoft.Json;

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

        public IActionResult Checkout()
        {

            //Get cart items
            var customerCart = ShoppingCart.GetCart(this.HttpContext, _context);
            var cartViewModel = new ShoppingCartViewModel
            {
                CartItems = customerCart.GetCartItems(),
                CartTotal = customerCart.GetTotal()                
            };

            foreach (var item in cartViewModel.CartItems)
            {
                item.Product.Translations = item.Product.Translations.Where(x => x.Language == CultureInfo.CurrentCulture.TwoLetterISOLanguageName).ToList();
                cartViewModel.ProductName = item.Product.Translations.SingleOrDefault().ProductName;
                cartViewModel.Price = item.Product.Price;
                cartViewModel.ImageName = item.Product.ImageName;
                cartViewModel.Quantity = item.Count;
                cartViewModel.ProductId = item.ProductId;
            }  
            
                      

            var cartItems = new List<Dictionary<string, object>>();
            foreach (var item in cartViewModel.CartItems)
            {
                cartItems.Add(new Dictionary<string, object>
                {
                    { "reference", cartViewModel.ProductId.ToString() },
                    { "name", cartViewModel.ProductName },
                    { "quantity", cartViewModel.Quantity },
                    { "unit_price", (int)(item.Product.Price *100)},
                    { "discount_rate", 1000 },
                    { "tax_rate", 2500 },

                });
            }
        
            var cart = new Dictionary<string, object> { { "items", cartItems } };

            var data = new Dictionary<string, object>
        {
            { "cart", cart }
        };//

            //Edit "merchant details"
            var merchant = new Dictionary<string, object>
    {
        { "id", "5160" },
        { "back_to_store_uri", "http://localhost:2087/Products" },
        { "terms_uri", "http://localhost:2087/ShoppingCart/Terms" },
        {
            "checkout_uri",
            "http://localhost:2087/ShoppingCarts/Checkout"
        },
        {
            "confirmation_uri",
            "http://localhost:2087/ShoppingCarts/OrderConfirmation" +
            "?klarna_order_id={checkout.order.id}"
        },
        {
            "push_uri",
            "https://example.com/push.aspx" +
            "?klarna_order_id={checkout.order.id}"
        }
    };

            data.Add("purchase_country", "SE");
            data.Add("purchase_currency", "SEK");
            data.Add("locale", "sv-se");
            data.Add("merchant", merchant);

            KlarnaPayment kP = new KlarnaPayment();
            
            var guiSnippet = kP.CreateOrder(JsonConvert.SerializeObject(data));

            return View("Checkout", guiSnippet);
        }

        public IActionResult OrderConfirmed()
        {


            return View();
        }

        


    }
}