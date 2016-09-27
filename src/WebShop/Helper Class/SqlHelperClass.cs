using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WebShop.Models;

namespace WebShop.Helper_Class
{
    public class SqlHelperClass
    {
        private readonly WebShopRepository _context;
        private readonly string connectionstring = @"Server=(localdb)\mssqllocaldb;Database=WebShop.Products;Trusted_Connection=True;";

        public SqlHelperClass(WebShopRepository context)
        {
            _context = context;
        }

        public IQueryable<ProductViewModel> PopulateProductViewModel()
        {
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on
                        new { p.ProductId, second = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName }
                        equals new { pt.ProductId, second = pt.Language }
                        select new ProductViewModel
                        {
                            ProductId = p.ProductId,
                            ProductName = pt.ProductName,
                            ProductDescription = pt.ProductDescription,
                            ProductCategoryId = p.ProductCategoryId,
                            ProductCategory = p.ProductCategory,
                            Price = p.Price,
                            ImageName = p.ImageName
                        };

            return query;
        }


        public AllProductData PopulateAllProductData(int id)
        {
            AllProductData productData;
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on
                        new { first = id, second = "en"}
                        equals new { first = pt.ProductId, second = pt.Language }
                        select new AllProductData
                        {
                            ProductId = p.ProductId,
                            ProductName = pt.ProductName,
                            ProductDescription = pt.ProductDescription,
                            ProductCategoryId = p.ProductCategoryId,
                            ProductCategory = p.ProductCategory,
                            Price = p.Price,
                            ImageName = p.ImageName
                        };
            productData = query.First(pro => pro.ProductId == id);

            query = from pt in _context.ProductTranslations
                    where (pt.ProductId == id && pt.Language == "sv") 
                    
                    select new AllProductData
                    {
                        ProductNameSV = pt.ProductName,
                        ProductDescriptionSV = pt.ProductDescription,
                        
                    };

            productData.ProductNameSV = query.FirstOrDefault().ProductNameSV;
            productData.ProductDescriptionSV = query.FirstOrDefault().ProductDescriptionSV;

            return productData;
        }

        public bool InsertProduct(AllProductData productData)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Products (Price,ProductCategoryId,ImageName) VALUES (@Price, @ProductCategoryId, @ImageName) SET @ID = SCOPE_IDENTITY()");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Price", productData.Price);
                    cmd.Parameters.AddWithValue("@ProductCategoryId", productData.ProductCategoryId);
                    cmd.Parameters.AddWithValue("@ImageName", productData.ImageName);
                    cmd.Parameters.Add("@ID", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                    connection.Open();
                    cmd.ExecuteNonQuery();

                    if (!InsertLanguage(cmd.Parameters["@ID"].Value.ToString(),"en", productData.ProductName, productData.ProductDescription))
                    {
                        //errorlog("Fel vid tillägg av masterlanguage");
                        return false;
                    }
                    if (!InsertLanguage(cmd.Parameters["@ID"].Value.ToString(), "sv", productData.ProductNameSV, productData.ProductDescriptionSV))
                    {
                        //errorlog("Fel vid tillägg av svenska");
                        return false;
                    }
                }
            }
            catch
            {
                //errorlog("Fel i InsertProduct");
                return false;
            }

            
            return true;
        }

        protected bool InsertLanguage(string id, string lang, string name, string desc)
        {
            int intId;
            if (!Int32.TryParse(id, out intId))
                return false;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO ProductTranslations (ProductId, Language, ProductName, ProductDescription) VALUES (@ProductId, @Language, @ProductName, @ProductDescription)");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@ProductId", id);
                    cmd.Parameters.AddWithValue("@Language", lang);
                    cmd.Parameters.AddWithValue("@ProductName", name);
                    cmd.Parameters.AddWithValue("@ProductDescription", desc);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool UpdateProduct(AllProductData productData)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Products SET Price = @Price, ProductCategoryId = @ProductCategoryId, ImageName=@ImageName WHERE ProductId = @ID");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Price", productData.Price);
                    cmd.Parameters.AddWithValue("@ProductCategoryId", productData.ProductCategoryId);
                    cmd.Parameters.AddWithValue("@ImageName", productData.ImageName);
                    cmd.Parameters.AddWithValue("@ID", productData.ProductId);
                    connection.Open();
                    cmd.ExecuteNonQuery();

                    if (!UpdateLanguage(productData.ProductId, "en", productData.ProductName, productData.ProductDescription))
                    {
                        //errorlog("Fel vid tillägg av masterlanguage");
                        return false;
                    }
                    if (!UpdateLanguage(productData.ProductId, "sv", productData.ProductNameSV, productData.ProductDescriptionSV))
                    {
                        //errorlog("Fel vid tillägg av svenska");
                        return false;
                    }
                }
            }
            catch
            {
                //errorlog("Fel i InsertProduct");
                return false;
            }


            return true;
        }

        protected bool UpdateLanguage(int id, string lang, string name, string desc)
        {
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    SqlCommand cmd = new SqlCommand("Update ProductTranslations SET ProductName = @ProductName, ProductDescription = @ProductDescription WHERE ProductId = @ProductId AND Language = @Language");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@ProductId", id);
                    cmd.Parameters.AddWithValue("@Language", lang);
                    cmd.Parameters.AddWithValue("@ProductName", name);
                    cmd.Parameters.AddWithValue("@ProductDescription", desc);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool DeleteProduct(int id)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Products WHERE ProductId = @ProductId");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@ProductId", id);
                    connection.Open();
                    cmd.ExecuteNonQuery();

                    SqlCommand cmd2 = new SqlCommand("DELETE FROM ProductTranslations WHERE ProductId = @ProductId");
                    cmd2.CommandType = CommandType.Text;
                    cmd2.Connection = connection;
                    cmd2.Parameters.AddWithValue("@ProductId", id);
                    connection.Open();
                    cmd2.ExecuteNonQuery();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public ShoppingCartViewModel GetProAndTransl(int id)
        {
            ShoppingCartViewModel _shoppingCartViewModel;
            var query = from p in _context.Products
                    where (p.ProductId == id)

                    select new ShoppingCartViewModel
                    {
                        Price = p.Price,
                        ImageName = p.ImageName

                    };

            _shoppingCartViewModel = query.FirstOrDefault();
            _shoppingCartViewModel.Price = query.FirstOrDefault().Price;
            _shoppingCartViewModel.ImageName = query.FirstOrDefault().ImageName;

            query = from pt in _context.ProductTranslations
                    where (pt.ProductId == id)
                    select new ShoppingCartViewModel
                    {
                        ProductName = pt.ProductName
                    };

            _shoppingCartViewModel.ProductName = query.FirstOrDefault().ProductName;

            return _shoppingCartViewModel;

        }

    }
}
