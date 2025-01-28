using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class CategoryController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();

            return View(categories);
        }

        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            //This is Server Side Validation 

            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Display Order Can not excatly Match the Name");

            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Category Created Successfuly";


                return RedirectToAction(nameof(Index));
            }


            return View(obj);
        }



        [HttpGet]

        public IActionResult Edit(int Id)
        {
            Category? category = _unitOfWork.Category.Get(i => i.Id == Id);

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {



            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Category Edited Successfuly";


                return RedirectToAction(nameof(Index));
            }


            return Content("Can not added");
        }




        [HttpGet]

        public IActionResult Delete(int Id)
        {
            Category? category = _unitOfWork.Category.Get(i => i.Id == Id);

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Delete(Category obj)
        {


            if (obj != null)
            {

                _unitOfWork.Category.Remove(obj);
                _unitOfWork.Save();

                TempData["Success"] = "Category Deleted Successfuly";

                return RedirectToAction(nameof(Index));
            }



            return Content("Can not deleted");


        }


    }
}
