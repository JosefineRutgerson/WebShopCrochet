using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebShop.Models
{
    public class ProductTranslation
    {
        public int ProductId { get; set; }
        //[Required(ErrorMessage ="Specify a language")]
        public string Language { get; set; }

        [Required(ErrorMessage = "Write a name for the product")]
        [Display(Name = "Product", ResourceType = typeof(Resources.Resource))]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Write a description")]
        [Display(Name = "Description", ResourceType = typeof(Resources.Resource))]
        public string ProductDescription { get; set; }


    }
}
