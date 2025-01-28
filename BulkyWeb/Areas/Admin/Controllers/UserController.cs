using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModel;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace BulkyBookWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class UserController : Controller
    {

        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUnitOfWork unitofWrok;


        public UserController(IUnitOfWork _unitOfWork,UserManager<IdentityUser> _userManger, RoleManager<IdentityRole> _roleManager)
        {
            unitofWrok = _unitOfWork;
            userManager = _userManger;
            roleManager = _roleManager;
        }

        public IActionResult Index()
        { 
            return View();              
        }

		public IActionResult RoleManagment( string userId)
		{


            RoleManagmentVM RoleVM = new RoleManagmentVM()
            {
                applicationUser = unitofWrok.ApplicationUser.Get(u => u.Id == userId, IncludeProperites: "Company"),
                RoleList = roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name, 
                    Value = i.Name
                }),
				CompanyList = unitofWrok.Company.GetAll().Select(i => new SelectListItem
				{
					Text = i.Name,
					Value = i.Id.ToString(),
				}),
			};

            RoleVM.applicationUser.Role = userManager.GetRolesAsync(unitofWrok.ApplicationUser.Get(u => u.Id == userId)).GetAwaiter().GetResult().FirstOrDefault();
			return View(RoleVM);
		}

        [HttpPost]
		public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM)
		{
		
            string OldRole = userManager.GetRolesAsync(unitofWrok.ApplicationUser.Get(u => u.Id == roleManagmentVM.applicationUser.Id)).GetAwaiter().GetResult().FirstOrDefault();


            ApplicationUser applicationUser = unitofWrok.ApplicationUser.Get(u => u.Id == roleManagmentVM.applicationUser.Id);    



            if (!(roleManagmentVM.applicationUser.Role == OldRole))
            {
                // a role was updated
                if(roleManagmentVM.applicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagmentVM.applicationUser.CompanyId;
                }
                if (OldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                
                unitofWrok.ApplicationUser.Update(applicationUser);
                unitofWrok.Save();


                userManager.RemoveFromRoleAsync(applicationUser , OldRole).GetAwaiter().GetResult();
                userManager.AddToRoleAsync(applicationUser , roleManagmentVM.applicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                if (OldRole ==SD.Role_Company && applicationUser.CompanyId != roleManagmentVM.applicationUser.CompanyId)
                {
                    applicationUser.CompanyId =roleManagmentVM.applicationUser.CompanyId ;
                    unitofWrok.ApplicationUser.Update(applicationUser);
                    unitofWrok.Save();  
                }
            }

            return RedirectToAction("Index");
		}






		#region APICalls
		[HttpGet]

        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = unitofWrok.ApplicationUser.GetAll(IncludeProperites: "Company").ToList();



            foreach (var user in objUserList)
            {
                user.Role = userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

                if(user.Company == null)
                {
                    user.Company = new Company { Name = "" };
                }

            }

            return Json(new { data = objUserList });

        }





        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {

            var objFromDb = unitofWrok.ApplicationUser.Get(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Erorr while Locking/Unlocking" });

            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            unitofWrok.ApplicationUser.Update(objFromDb);
            unitofWrok.Save();

            return Json(new { success = true, message = "Operation Successfully" });




		}


		#endregion

	}
}
