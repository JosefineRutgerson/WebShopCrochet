using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebShop.Models
{
    public class ProductCategory
    {
        [Required]
        [Display(Name ="Category Id")]
        public int ProductCategoryId { get; set; }
        [Required]
        [Display(Name ="Category Name")]
        public string ProductCategoryName { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
