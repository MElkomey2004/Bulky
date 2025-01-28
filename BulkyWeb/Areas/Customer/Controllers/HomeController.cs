using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{

    [Area("Customer")]
	[Authorize(Roles = SD.Role_Customer)]
	public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitofwork)
        {
            _logger = logger;
            _unitOfWork = unitofwork;
        }

        public IActionResult Index()
        {


            IEnumerable<Product> proudctList = _unitOfWork.Product.GetAll(IncludeProperites: "Category,ProductImages");

            return View(proudctList);
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            ShoppingCart cart = new() 
            {
                Product = _unitOfWork.Product.Get(u => u.Id == id, IncludeProperites: "Category,ProductImages"),

                Count = 1,
                ProductId = id
            };


     
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {

          var claimsIdentity = (ClaimsIdentity)User.Identity;        
          var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


           shoppingCart.ApplicationUserId = UserId;
            shoppingCart.Id = 0;

            ShoppingCart shopcartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId  == UserId && u.ProductId == shoppingCart.ProductId);

            if(shopcartFromDb != null)
            {
				//shopping Cart Exists
				shopcartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(shopcartFromDb);
                _unitOfWork.Save();
            }
            else
            {
				//Add Card Record
				_unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart ,_unitOfWork.ShoppingCart.
                   GetAll(u => u.ApplicationUserId == UserId).Count());
			}
            TempData["success"] = "Cart updated successfully";
        

            return RedirectToAction("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
