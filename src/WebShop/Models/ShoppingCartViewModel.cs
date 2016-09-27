using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebShop.Models
{
    public class ShoppingCartViewModel
    {

        public string  ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ImageName { get; set; }
        public List<Cart> CartItems { get; set; }
        public decimal CartTotal { get; set; }

    }
}
