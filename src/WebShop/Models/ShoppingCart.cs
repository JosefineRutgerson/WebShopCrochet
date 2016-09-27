using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace WebShop.Models
{
    public class ShoppingCart
    {
        string ShoppingCartId { get; set; }
        private readonly WebShopRepository _context;

        public ShoppingCart(WebShopRepository context, HttpContext httpcontext)
        {
            _context = context;

        }

        public static ShoppingCart GetCart(HttpContext httpContext, WebShopRepository context)
        {
            var cart = new ShoppingCart(context, httpContext);
            cart.ShoppingCartId = cart.GetCartId(httpContext, context);
            return cart;
        }
        // Helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(Controller controller, WebShopRepository webContext)
        {
            return GetCart(controller.HttpContext, webContext);
        }

        // Adds products to cart
        public void AddToCart(Product product)
        {
            // Get the matching cart and product instances
            var cartItem = _context.Carts.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.ProductId == product.ProductId);

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new Cart
                {
                    ProductId = product.ProductId,
                    CartId = ShoppingCartId,
                    Count = 1
                };
                _context.Carts.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, 
                // then add one to the quantity
                cartItem.Count++;
            }
            // Save changes
            _context.SaveChanges();
        }

        //removes products from cart
        public int RemoveFromCart(int id)
        {
            // Get the cart
            var cartItem = _context.Carts.Single(
                cart => cart.CartId == ShoppingCartId
                && cart.Id == id);

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    _context.Carts.Remove(cartItem);
                }
                // Save changes
                _context.SaveChanges();
            }
            return itemCount;
        }
        public void EmptyCart()
        {
            var cartItems = _context.Carts.Where(
                cart => cart.CartId == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                _context.Carts.Remove(cartItem);
            }
            // Save changes
            _context.SaveChanges();
        }

        public List<Cart> GetCartItems()
        {
            return _context.Carts.Where(
                cart => cart.CartId == ShoppingCartId).Include(pro => pro.Product.Translations).ToList();

        }

        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in _context.Carts
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Count).Sum();
            // Return 0 if all entries are null
            return count ?? 0;
        }
        public decimal GetTotal()
        {
            // Multiply album price by count of that album to get 
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total
            decimal? total = (from cartItems in _context.Carts
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.Count *
                              cartItems.ProductId).Sum();  // Fixa detta!

            return total ?? decimal.Zero;
        }
        //public int CreateOrder(Order order)
        //{
        //    decimal orderTotal = 0;

        //    var cartItems = GetCartItems();
        //    // Iterate over the items in the cart, 
        //    // adding the order details for each
        //    foreach (var item in cartItems)
        //    {
        //        var orderDetail = new OrderDetail
        //        {
        //            AlbumId = item.AlbumId,
        //            OrderId = order.OrderId,
        //            UnitPrice = item.Album.Price,
        //            Quantity = item.Count
        //        };
        //        // Set the order total of the shopping cart
        //        orderTotal += (item.Count * item.Album.Price);

        //        storeDB.OrderDetails.Add(orderDetail);

        //    }
        //    // Set the order's total to the orderTotal count
        //    order.Total = orderTotal;

        //    // Save the order
        //    storeDB.SaveChanges();
        //    // Empty the shopping cart
        //    EmptyCart();
        //    // Return the OrderId as the confirmation number
        //    return order.OrderId;
        //}
        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContext httpContext, WebShopRepository _context)
        {
            string safevalue;
            if (httpContext.Request.Cookies["cart"] != null)
            {
                var value = httpContext.Request.Cookies["cart"];
                //Applicera en HtmlEncode för att inte riskera få in
                //tex javascriptkod till sidan.
                safevalue = HtmlEncoder.Default.Encode(value);
                
            }
            else
            {
                safevalue = Guid.NewGuid().ToString();
                
            }

            //Första besöket eller inga cookies påslagna.
            httpContext.Response.Cookies.Append(
            "cart",
            safevalue,
            new Microsoft.AspNetCore.Http.CookieOptions()
            {
                Path = "/",
                HttpOnly = false,  //True == client js can't read.
                    Secure = false,     //True == only https
                    Expires = DateTime.Now.AddMonths(6)  //Cookien sparas i 6 månader framåt.  DateTime.Now.AddDays(-1) tar bort cookien.
                });

            return safevalue;

        }
       
        //When a user has logged in, migrate their shopping cart to be associated with their username
        public void MigrateCart(string userName)
        {
            var shoppingCart = _context.Carts.Where(
                c => c.CartId == ShoppingCartId);

            foreach (Cart item in shoppingCart)
            {
                item.CartId = userName;
            }
            _context.SaveChanges();
        }
    }
}

    

