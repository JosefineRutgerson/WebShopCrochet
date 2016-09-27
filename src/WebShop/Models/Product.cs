using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebShop.Models
{
    public class Product
    {
        //[Required(ErrorMessage = "Write a number")]
        //[Display(Name = "Productnumber", ResourceType = typeof(Resources.Resource))]
        //[Key]
        public int ProductId { get; set; }

        [Display(Name = "Price", ResourceType = typeof(Resources.Resource))]
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "Write a price for the product")]
        public decimal Price { get; set; }

        public int ProductCategoryId { get; set; }

        //[Required(ErrorMessage = "Chose a category")]
        //[Display(Name = "ProductCategory", ResourceType = typeof(Resources.Resource))]
        public ProductCategory ProductCategory { get; set; }

        public string ImageName { get; set; }

        
        public virtual ICollection<ProductTranslation> Translations { get; set; }
    }
}
