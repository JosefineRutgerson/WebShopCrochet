using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebShop.Models;

namespace WebShop.Models
{
    public class AllProductData : ProductViewModel
    {
        public string LanguageSV { get; set; }
        [Display(Name="ProductNameSV", ResourceType = typeof(Resources.Resource)) ]
        public string ProductNameSV { get; set; }
        [Display(Name = "ProductDescSV", ResourceType = typeof(Resources.Resource))]
        public string ProductDescriptionSV { get; set; }
    }
}
