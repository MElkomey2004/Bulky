using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModel;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize]

    public class OrderController : Controller
	{

        private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public OrderVM OrderVm {get ;set;}

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
		{
			return View();
		}


		public IActionResult Details(int orderId)
		{
			 OrderVm = new()
			{
				orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, IncludeProperites: "applicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, IncludeProperites: "Product")


			};
			return View(OrderVm);
		}
		[HttpPost]
		[Authorize(Roles =SD.Role_Admin +","+ SD.Role_Employee)]
        
		public IActionResult UpdateOrderDetail()
		{
			OrderHeader orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVm.orderHeader.Id);

			orderHeaderFromDb.Name = OrderVm.orderHeader.Name;
			orderHeaderFromDb.PhoneNumber = OrderVm.orderHeader.PhoneNumber;
			orderHeaderFromDb.StreetAddress = OrderVm.orderHeader.StreetAddress;
			orderHeaderFromDb.City = OrderVm.orderHeader.City;
			orderHeaderFromDb.State = OrderVm.orderHeader.State;
			orderHeaderFromDb.PostalCode = OrderVm.orderHeader.PostalCode;

			if (!string.IsNullOrEmpty(OrderVm.orderHeader.Carrier))
			{
				orderHeaderFromDb.Carrier = OrderVm.orderHeader.Carrier;


            }
            if (!string.IsNullOrEmpty(OrderVm.orderHeader.TrakingNumber))
            {
                orderHeaderFromDb.TrakingNumber = OrderVm.orderHeader.TrakingNumber;


            }


			_unitOfWork.OrderHeader.Update(orderHeaderFromDb);
			_unitOfWork.Save();

			TempData["Success"] = "Order Details Updated Successfully";

			return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }


		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]


		public IActionResult StratProcessing()
		{
			_unitOfWork.OrderHeader.UpdateStatus(OrderVm.orderHeader.Id, SD.StatusInProcess);
			_unitOfWork.Save();


			TempData["Success"] = "Order Details Updated Successfully";
			return RedirectToAction(nameof(Details), new { orderId = OrderVm.orderHeader.Id });
		}

		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

		public IActionResult ShipOrder()
		{

			var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVm.orderHeader.Id);
			orderHeaderFromDb.TrakingNumber = OrderVm.orderHeader.TrakingNumber;
			orderHeaderFromDb.Carrier = OrderVm.orderHeader.Carrier;
			orderHeaderFromDb.OrderStatus = SD.StatusShipped;
			orderHeaderFromDb.ShoppingDate = DateTime.Now;
			if(orderHeaderFromDb.PaymentStatus == SD.PaymentStatusDelayPayment)
			{
				orderHeaderFromDb.PaymentDueDate = DateTime.Now.AddDays(30);

			}



			_unitOfWork.OrderHeader.Update(orderHeaderFromDb);
			_unitOfWork.Save();


			TempData["Success"] = "Order Shipped Successfully";
			return RedirectToAction(nameof(Details), new { orderId = OrderVm.orderHeader.Id });
		}


		//There is Erorr here because i don't code in the video because the stripe payment not uses in Egpyt and I Search and Get solve the proble.
		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult CancelOrder()
		{
			var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVm.orderHeader.Id);

			_unitOfWork.OrderHeader.UpdateStatus(orderHeaderFromDb.Id , SD.StatusCancelled , SD.StatusCancelled);

			_unitOfWork.Save();


			TempData["Success"] = "Order Cancelled Successfully";
			return RedirectToAction(nameof(Details), new { orderId = OrderVm.orderHeader.Id });
		}

		[ActionName("Details")]
		[HttpPost]
		//Not Work Because the stripe not work in egypt i search about payment and alter to this code (Eng: Mohamed Wael Elkomey)
		public IActionResult Details_PAY_NOW()
		{


			//OrderVm.orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVm.orderHeader.Id, IncludeProperites: "applicationUser"),
			//OrderVm.OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == OrderVm.orderHeader.Id, IncludeProperites: "Product")
			//
			//return RedirectToAction(nameof(Details), new { orderId = OrderVm.orderHeader.Id });

			return Content("Not Work Because the stripe not work in egypt i search about payment and alter to this code (Eng: Mohamed Wael Elkomey)") ;

		}

		#region APICalls
		[HttpGet]

		//This is Api Endpoint.
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderHeaders ;

			if(User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
			{
				orderHeaders = _unitOfWork.OrderHeader.GetAll(IncludeProperites: "applicationUser").ToList();

			}
			else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

				orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId, IncludeProperites: "applicationUser");



			}

			
			switch (status)
			{
				case "pending":
					orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayPayment);
					break;

				case "inprocess":
					orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
					break;

				case "completed":
					orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
					break;

				case "approved":
					orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
					break;

				default:
					break;
			}

			return Json(new { data = orderHeaders });

		}





		#endregion
	}
}
