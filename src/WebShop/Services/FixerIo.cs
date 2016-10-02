using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Services
{
    public class FixerIo
    {
        private static HttpClient client = new HttpClient();

        public FixerIo()
        {

        }

        public async Task<JObject> GetLatestRates()
        {
            string url = "http://api.fixer.io/latest";
                
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var root = JObject.Parse(responseBody);
            var rates = root.Value<JObject>("rates");

            var baseRate = root.Value<string>("base");
            rates.Add(baseRate, 1.00);

            return rates;       
            
        }

        private decimal GetRate(string from, string to)
        {
            JObject rates = GetLatestRates().Result;

            from = from.ToUpper();
            to = to.ToUpper();

            var fromRates = rates.Value<decimal>(from);
            var toRate = rates.Value<decimal>(to);

            decimal rate = toRate / fromRates;


            return rate;

        }

        public decimal ConvertPrice(string from, string to, decimal currentPrice)
        {
            return currentPrice * GetRate(from, to);
        }


        
        

    }    

}

