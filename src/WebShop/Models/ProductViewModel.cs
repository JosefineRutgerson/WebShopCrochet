using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WebShop.Helper_Class;

namespace WebShop.Models
{
    public class ProductViewModel
    {
        [Required(ErrorMessage = "Write a number")]
        [Display(Name = "Productnumber", ResourceType = typeof(Resources.Resource))]
        public int ProductId { get; set; }

        //[Required(ErrorMessage = "Specify a language")]
        public string Language { get; set; }

        public int ProductCategoryId { get; set; }

        //[Required(ErrorMessage = "Chose a category")]
        [Display(Name ="ProductCategory", ResourceType = typeof(Resources.Resource))]
        public ProductCategory ProductCategory { get; set; }

        [Required(ErrorMessage = "Write a name for the product")]
        [Display(Name = "Product", ResourceType = typeof(Resources.Resource))]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Write a description")]
        [Display(Name = "Description", ResourceType = typeof(Resources.Resource))]
        public string ProductDescription { get; set; }

        [Display(Name = "ImageName", ResourceType = typeof(Resources.Resource))]
        public string ImageName { get; set; }

        [Display(Name = "Price", ResourceType = typeof(Resources.Resource))]
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "Write a price for the product")]
        public decimal Price { get; set; }

        //private readonly WebShopRepository _context = new WebShopRepository();
        //MergeTableQuery mergeTableQuery;

        //public ProductViewModel()
        //{
        //    //_context = context;
        //    //mergeTableQuery = new MergeTableQuery(_context);

        //}

        //public ProductViewModel ReturnProductViewModel()
        //{
        //    MergeTableQuery mergeTableQuery = new MergeTableQuery();
        //    ProductViewModel productViewModel = mergeTableQuery.PopulateProductViewModel();
        //}




    }
}
