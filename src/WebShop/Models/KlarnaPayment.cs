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
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebShop.Models
{
    public class KlarnaPayment
    {
        FixerIo exchangeRate;
        private readonly WebShopRepository _context;
        string shared_secret = "tE94QeKzSdUVJGe";

        public KlarnaPayment()
        {

        }

        public List<Dictionary<string, object>> AddItemsToKlarna(FixerIo rate)
        {
            exchangeRate = rate;
            //ShoppingCart _shoppingCart = ShoppingCart.GetCart(ShoppingCart.HttpContext, _context);
            //var shoppingcartList = _shoppingCart.GetCartItems();   
            var cartItems = new List<Dictionary<string, object>>();
            return cartItems;
        }

        private string CreateAuthorization(string data)
        {
            //base64(hex(sha256 (request_payload + shared_secret)))

            using (var algorithm = SHA256.Create())
            {
                var hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(data));
                var base64 = System.Convert.ToBase64String(hash);
                return base64;
            }
        }

        public string CreateOrder(string jsonData)
        {   //Create Post request
            HttpClient _client = new HttpClient();

            HttpRequestMessage message = new HttpRequestMessage();
            message.RequestUri = new Uri("https://checkout.testdrive.klarna.com/checkout/orders");
            message.Method = HttpMethod.Post;
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.klarna.checkout.aggregated-order-v2+json"));
            message.Headers.Authorization = new AuthenticationHeaderValue("Klarna", CreateAuthorization(jsonData + shared_secret));

            message.Content = new StringContent(jsonData, Encoding.UTF8, "application/vnd.klarna.checkout.aggregated-order-v2+json");

            var response = _client.SendAsync(message).Result;

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var location = response.Headers.Location.AbsoluteUri;

                //Get order
                HttpRequestMessage getMessage = new HttpRequestMessage();
                getMessage.RequestUri = new Uri(location);
                getMessage.Method = HttpMethod.Get;
                getMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.klarna.checkout.aggregated-order-v2+json"));
                getMessage.Headers.Authorization = new AuthenticationHeaderValue("Klarna", CreateAuthorization(shared_secret));

                var getResponse = _client.SendAsync(getMessage).Result;
                var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;

                var gui = JsonConvert.DeserializeObject<KlarnaGetCartResponse>(getResponseBody);
                var obj = JObject.Parse(getResponseBody);
                return gui.gui.snippet;


            }

            return response.Content.ReadAsStringAsync().Result;
        }
        public class Gui
        {
            public string layout { get; set; }
            public string snippet { get; set; }
        } 


        public class KlarnaGetCartResponse
        {
            public string status { get; set; }
            public string id { get; set; }
            public Gui gui { get; set; }
        }
    
    }
}
