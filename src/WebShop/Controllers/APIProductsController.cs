using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebShop.Models;
using WebShop.Helper_Class;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebShop.Controllers
{
    [Route("api/[controller]")]
    public class APIProductsController : Controller
    {
        private SqlHelperClass sqlHelper;
        WebShopRepository _context;
        public APIProductsController(WebShopRepository context)
        {
            WebShopRepository = context;
            sqlHelper = new SqlHelperClass(WebShopRepository);
        }
        public WebShopRepository WebShopRepository { get; set; }

        [HttpGet]
        public IEnumerable<ProductViewModel> GetAll()
        {
            var productList = sqlHelper.PopulateProductViewModel();

            return productList.ToList();
        }

        [HttpGet("{id}", Name = "GetProduct")]
        public IActionResult GetById(int id)
        {
            var productList = sqlHelper.PopulateProductViewModel().ToList();
            ProductViewModel product = new ProductViewModel();
            if (productList == null)
            {
                return NotFound();
            
            }

            //var item = WebShopRepository.Products.Where(pro => pro.ProductId == id);
            foreach (var item in productList)
            {
                if (item.ProductId == id)
                {
                    product = item;
                    
                }
                    
            }
                return new ObjectResult(product);
        }

        [HttpPost]
        public IActionResult Create([FromBody] AllProductData item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            sqlHelper.InsertProduct(item);
            return CreatedAtRoute("Getproduct", new { id = item.ProductId }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int? id, [FromBody] AllProductData item)
        {
            if (item == null || item.ProductId != id)
            {
                return BadRequest();
            }

            item.ProductId = id ?? default(int);
            if (ModelState.IsValid)
            {
                try
                {
                    sqlHelper.UpdateProduct(item);
                    //_context.Update(productData);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(item.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
                return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var chosenPro = WebShopRepository.Products.Where(pro => pro.ProductId == id);
            if (chosenPro == null)
            {
                return NotFound();
            }

            sqlHelper.DeleteProduct(id);
            return new NoContentResult();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
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
