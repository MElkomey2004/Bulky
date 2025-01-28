using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModel;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class ProductController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        private readonly IWebHostEnvironment _webHostEnvironment;   
        public ProductController(IUnitOfWork unitOfWork , IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;   
        }

        public IActionResult Index()
        {
            List<Product> Products = _unitOfWork.Product.GetAll(IncludeProperites:"Category").ToList();

            return View(Products);
        }

        public IActionResult Upsert(int ? Id)
        {
            //IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            //{
            //    Text = u.Name,
            //    Value = u.Id.ToString(),
            //});  // This Is Operation Porjection

            //ViewBag.CategoryList = CategoryList;

            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}), // This Is Operation Porjection


		        Product = new Product(),
            };

            if(Id == null || Id == 0){

                //Create

                return View(productVM);
            }

            else
            {
               
                //Update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == Id , IncludeProperites:"ProductImages");

                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM obj , List<IFormFile> files)
        {
            //This is Server Side Validation 

            if (ModelState.IsValid)
            {
               

				if (obj.Product.Id == 0)
				{

					_unitOfWork.Product.Add(obj.Product);
				}
				else
				{
					_unitOfWork.Product.Update(obj.Product);
				}
				_unitOfWork.Save();

				string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(files != null)
                {
                    foreach(IFormFile file in files)
                    {
                        string FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                        string ProductPath = @"images\products\product-" + obj.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, ProductPath);

                        if(!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        using (var fileStream = new FileStream(Path.Combine(finalPath, FileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + ProductPath + @"\" + FileName,
                            ProductId = obj.Product.Id,

                        };

                        if(obj.Product.ProductImages == null)
                        {
                            obj.Product.ProductImages = new List<ProductImage>();
                        }

                        obj.Product.ProductImages.Add(productImage);
                        _unitOfWork.ProductImage.Add(productImage);
                          
                    }


                    _unitOfWork.Product.Update(obj.Product);
                    _unitOfWork.Save();

					//if (!string.IsNullOrEmpty(obj.Product.ImageUrl))
					//{
					//    var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));



					//    if (System.IO.File.Exists(oldImagePath))
					//    {
					//        System.IO.File.Delete(oldImagePath);
					//    }
					//}
					//using (var fileStream = new FileStream(Path.Combine(ProductPath, FileName), FileMode.Create))
					//{
					//    file.CopyTo(fileStream);
					//}

					//obj.Product.ImageUrl = @"\Images\Product\" + FileName;

				}

                TempData["Success"] = "Product Created/Updated Successfuly";


                return RedirectToAction(nameof(Index));
            }

            else
            {



                obj.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });// This Is Operation Porjection


				
				

				
                return View(obj);
			}


        }



        //[HttpGet]

        //public IActionResult Edit(int Id)
        //{
        //    Product? product = _unitOfWork.Product.Get(i => i.Id == Id);

        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}

        //[HttpPost]
        //public IActionResult Edit(Product obj)
        //{



        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(obj);
        //        _unitOfWork.Save();
        //        TempData["Success"] = "Product Edited Successfuly";


        //        return RedirectToAction(nameof(Index));
        //    }


        //    return Content("Can not added");
        //}




        //[HttpGet]

        //public IActionResult Delete(int Id)
        //{
        //    Product? product = _unitOfWork.Product.Get(i => i.Id == Id);

        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}

        //[HttpPost]
        //public IActionResult Delete(Product obj)
        //{


        //    if (obj != null)
        //    {

        //        _unitOfWork.Product.Remove(obj);
        //        _unitOfWork.Save();

        //        TempData["Success"] = "Product Deleted Successfuly";

        //        return RedirectToAction(nameof(Index));
        //    }



        //    return Content("Can not deleted");


        //}



        public IActionResult DeleteImage(int imageId)
        {
            var imageToDeBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
            var productId = imageToDeBeDeleted.ProductId;

            if(imageToDeBeDeleted != null)
            {
                if (!String.IsNullOrEmpty(imageToDeBeDeleted.ImageUrl))
                {

                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageToDeBeDeleted.ImageUrl.TrimStart('\\'));



                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _unitOfWork.ProductImage.Remove(imageToDeBeDeleted);
                _unitOfWork.Save();

                TempData["success"] = "Deleted Successfully";
			}
            return RedirectToAction(nameof(Upsert) , new {id = productId });
        }



		#region APICalls
		[HttpGet]

        public IActionResult GetAll()
        {
            List<Product> Products = _unitOfWork.Product.GetAll(IncludeProperites: "Category").ToList();

            return Json(new { data = Products });

        }





        [HttpDelete]
        public IActionResult Delete(int? id)
        {

            var productDeleted = _unitOfWork.Product.Get(u => u.Id == id);

            if (productDeleted == null)
            {
                return Json(new { success = false, message = "Erorr While Deleting" });
            }


			string ProductPath = @"images\products\product-" + id;
			string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, ProductPath);

			if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach(string filePath in filePaths) 
                {
                    System.IO.File.Delete(filePath);
                }

				Directory.Delete(finalPath);
            }


			_unitOfWork.Product.Remove(productDeleted);
            _unitOfWork.Save();

			return Json(new { success = true, message = "Deleted Successfully" });




		}
		#endregion

	}
}
