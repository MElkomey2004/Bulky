using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public  class ProductRepository : Repository<Product> , IProductRepository
    {
        private ApplicationDbContext _db;


        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;


        }

        public void Update(Product obj)
        {
           var oldObj =   _db.Products.FirstOrDefault(u => u.Id == obj.Id);

            if(oldObj != null)
            {
                oldObj.Title = obj.Title;
                oldObj.Description = obj.Description;
                oldObj.CategoryId = obj.CategoryId;
                oldObj.Author = obj.Author;
                oldObj.ISBN = obj.ISBN;
                oldObj.Price = obj.Price;
                oldObj.Price50 = obj.Price50;
                oldObj.Price100 = obj.Price100;
                oldObj.ListPrice = obj.ListPrice;
                oldObj.ProductImages = obj.ProductImages;
                
                //if(obj.ImageUrl  != null)
                //{
                //    oldObj.ImageUrl = obj.ImageUrl;
                //}
            }
        }
    }
}
