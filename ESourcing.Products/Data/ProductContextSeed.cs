using ESourcing.Products.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESourcing.Products.Data
{
    public class ProductContextSeed
    {
        public static void SeedData(IMongoCollection<Product> productCollection)
        {
            bool existProduct = productCollection.Find(p => true).Any();
            if(!existProduct)
            {
                productCollection.InsertManyAsync(GetConfigureProducts());
            }
        }

        private static IEnumerable<Product> GetConfigureProducts()
        {
            List<Product> productList = new List<Product>();
            for (int i = 0; i < 10; i++)
            {
                Product pro = new Product();
                pro.Name = i+" Name";
                pro.Summary = i + " Summary";
                pro.Description = i + " description";
                pro.Price = new Random().Next(100,10000);
                pro.ImageFile = "product_"+i;
                pro.Category = i+"_Smart";

                productList.Add(pro);
            }
            return productList;
        }
    }
}
