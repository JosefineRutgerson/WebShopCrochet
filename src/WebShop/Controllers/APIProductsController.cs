using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebShop.Models;
using WebShop.Helper_Class;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebShop.Controllers
{
    [Route("api/[controller]")]
    public class APIProductsController : Controller
    {
        private SqlHelperClass sqlHelperClas;
        WebShopRepository _context;
        public APIProductsController(WebShopRepository context)
        {
            WebShopRepository = context;
            sqlHelperClas = new SqlHelperClass(WebShopRepository);
        }
        public WebShopRepository WebShopRepository { get; set; }

        [HttpGet]
        public IEnumerable<ProductViewModel> GetAll()
        {
            var productList = sqlHelperClas.PopulateProductViewModel();

            return productList.ToList();
        }

        [HttpGet("{id}", Name = "GetProduct")]
        public IActionResult GetById(int id)
        {
            var item = WebShopRepository.Products.Where(pro => pro.ProductId == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }
    }

}

//// GET: api/values
//[HttpGet]
//public IEnumerable<string> Get()
//{
//    return new string[] { "value1", "value2" };
//}

//// GET api/values/5
//[HttpGet("{id}")]
//public string Get(int id)
//{
//    return "value";
//}

//// POST api/values
//[HttpPost]
//public void Post([FromBody]string value)
//{
//}

//// PUT api/values/5
//[HttpPut("{id}")]
//public void Put(int id, [FromBody]string value)
//{
//}

//// DELETE api/values/5
//[HttpDelete("{id}")]
//public void Delete(int id)
//{
//}
