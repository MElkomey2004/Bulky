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

    public class CompanyController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;


        public CompanyController(IUnitOfWork unitOfWork , IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
           
        }

        public IActionResult Index()
        {
            List<Company> Companys = _unitOfWork.Company.GetAll().ToList();

            return View(Companys);
        }

        public IActionResult Upsert(int ? Id)
        {
            //IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            //{
            //    Text = u.Name,
            //    Value = u.Id.ToString(),
            //});  // This Is Operation Porjection

            //ViewBag.CategoryList = CategoryList;

          

            if(Id == null || Id == 0){

                //Create

                return View(new Company());
            }

            else
            {
                //Update
                Company CompanyObj = _unitOfWork.Company.Get(u => u.Id == Id);

                return View(CompanyObj);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company CompanyObj)
        {
            //This is Server Side Validation 

            if (ModelState.IsValid)
            {
 
       
                if(CompanyObj.Id == 0)
                {

                    _unitOfWork.Company.Add(CompanyObj);
                }
                else
                {
                    _unitOfWork.Company.Update(CompanyObj);
                }
                _unitOfWork.Save();
                TempData["Success"] = "Company Created Successfuly";


                return RedirectToAction(nameof(Index));
            }

            else
            {


             


				
				

				
                return View(CompanyObj);
			}


        }








        #region APICalls
        [HttpGet]

        public IActionResult GetAll()
        {
            List<Company> companies = _unitOfWork.Company.GetAll().ToList();

            return Json(new { data = companies });

        }





        [HttpDelete]
        public IActionResult Delete(int? id)
        {

            var CompanyDeleted = _unitOfWork.Company.Get(u => u.Id == id);

            if (CompanyDeleted == null)
            {
                return Json(new { success = false, message = "Erorr While Deleting" });
            }

		

			

            _unitOfWork.Company.Remove(CompanyDeleted);
            _unitOfWork.Save();

			return Json(new { success = true, message = "Deleted Successfully" });




		}
		#endregion

	}
}
