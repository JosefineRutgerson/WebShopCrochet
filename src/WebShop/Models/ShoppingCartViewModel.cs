using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebShop.Models
{
    public class ShoppingCartViewModel
    {
        public int ProductId { get; set; }
        public string  ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ImageName { get; set; }
        public List<Cart> CartItems { get; set; }
        [DataType(DataType.Currency)]
        public decimal CartTotal { get; set; }

    }
}
