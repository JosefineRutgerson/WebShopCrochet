using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebShop.Models;

namespace WebShop.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly WebShopRepository _context;
        Product productmodel = new Product();

        public ShoppingCartViewComponent( WebShopRepository context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ShoppingCart _shoppingCart = ShoppingCart.GetCart(this.HttpContext, _context);
            int cartCount = _shoppingCart.GetCount();
            ViewData["CartCount"] = cartCount;
            return View();

        }

        //public int CartQuantity()
        //{

        //}

        //public async Task<IViewComponentResult> InvokeAsync(int id)
        //{
        //    var items = await GetItemsAsync(id);
        //    return View(items);
        //}
        //private Task<List<ProductViewModel>> GetItemsAsync(int id)
        //{
        //    //List<ProductViewModel> productViewModel = new List<ProductViewModel>();
        //    //productViewModel = _context.Products.Where(pro => pro.ProductId == id).ToList();
        //    //return productViewModel;
        //}

    }
}
