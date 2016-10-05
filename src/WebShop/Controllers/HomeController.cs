using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using WebShop.Interfaces;
using WebShop.ViewModels;
using WebShop.Models;
using System.Net.Http;

namespace WebShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly WebShopRepository _context;
        private readonly IDateTime _datetime;
        //public ShoppingCart shoppingCart = new ShoppingCart(_context, this.HttpContext);

        public HomeController(IDateTime datetime, WebShopRepository context)
        {
            _datetime = datetime;
            _context = context;
        }
        

        public IActionResult Index()
        {
            ShoppingCart _shoppingCart = ShoppingCart.GetCart(this.HttpContext, _context);
            int cartCount = _shoppingCart.GetCount();
            ViewData["CartCount"] = cartCount;
            return View();
        }

        public IActionResult About(int id, string name)
        {
            ViewData["Message"] = id.ToString() + " " + name + _datetime.Now;

            return View();
        }


        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        public IActionResult Contact()
        {
            ContactViewModel vmodel = new ContactViewModel();
            vmodel.CurrentDateAndTime = _datetime.Now.ToString();
            vmodel.Id = 0;
            List<string> list = new List<string>();
            vmodel.Names = list;
            list.Add("bosse");
            list.Add("bengan");

            return View(vmodel);
        }
        //public IActionResult Contact([FromServices]IDateTime _datetime)
        //{
        //    ViewData["Message"] = _datetime.Now;

        //    return View();
        //}

        public IActionResult Error()
        {
            return View();
        }
    }
}
