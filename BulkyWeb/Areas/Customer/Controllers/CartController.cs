using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModel;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace BulkyBookWeb.Areas.Customer.Controllers
{

	[Area("customer")]
	[Authorize]
	public class CartController : Controller

	{
		private readonly IUnitOfWork _unitOfWork;

		[BindProperty]

		public ShoppingCartVM shoppingCartVM { get; set; }

		public CartController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;

		}
		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;


			var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			shoppingCartVM = new()
			{
				ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId, IncludeProperites: "Product"),
				OrderHeader = new()

			};

			IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

			foreach (ShoppingCart cart in shoppingCartVM.ShoppingCartList)
			{
				cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.ProductId).ToList();	
				cart.Price = GetPriceBasedOnQuantity(cart);
				shoppingCartVM.OrderHeader.TotalOrder += (cart.Price * cart.Count);


			}
			return View(shoppingCartVM);
		}



		public IActionResult plus(int cartId)
		{
			var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);

			cartFromDb.Count += 1;
			_unitOfWork.ShoppingCart.Update(cartFromDb);
			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		public IActionResult minus(int cartId)
		{
			var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId , tracked: true);

			if (cartFromDb.Count <= 1)
			{

                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.
                     GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
                _unitOfWork.ShoppingCart.Remove(cartFromDb);

			}
			else
			{
				cartFromDb.Count -= 1;
				_unitOfWork.ShoppingCart.Update(cartFromDb);

			}

			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}


		public IActionResult Remove(int cartId)
		{
			var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId , tracked: true);

            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.
			GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
			_unitOfWork.Save();
            return RedirectToAction(nameof(Index));
		}


		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;


			var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			shoppingCartVM = new()
			{
				ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId, IncludeProperites: "Product"),
				OrderHeader = new()

			};

			shoppingCartVM.OrderHeader.applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == UserId);

			shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.applicationUser.Name;
			shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.applicationUser.PhoneNumber;
			shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.applicationUser.StreetAdress;
			shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.applicationUser.City;
			shoppingCartVM.OrderHeader.State = shoppingCartVM.OrderHeader.applicationUser.State;
			shoppingCartVM.OrderHeader.PostalCode = shoppingCartVM.OrderHeader.applicationUser.PostalCode;

			foreach (ShoppingCart cart in shoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				shoppingCartVM.OrderHeader.TotalOrder += (cart.Price * cart.Count);


			}


			return View(shoppingCartVM);
		}




		[HttpPost]
		[ActionName("Summary")]
		public IActionResult SummaryPost()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;


			var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;




			shoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId, IncludeProperites: "Product");

			shoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
			shoppingCartVM.OrderHeader.ApplicationUserId = UserId;


			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == UserId);



			foreach (ShoppingCart cart in shoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				shoppingCartVM.OrderHeader.TotalOrder += (cart.Price * cart.Count);


			}

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				//it's a regluar coustomer 
				shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
				shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
			}
			else
			{
				//it's a company user
				shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayPayment;
				shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
			}

			shoppingCartVM.OrderHeader.Id = 0;

			_unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
			_unitOfWork.Save();

			foreach (var cart in shoppingCartVM.ShoppingCartList)
			{
				OrderDetail OrderDetail = new()
				{
					ProductId = cart.ProductId,
					OrderHeaderId = shoppingCartVM.OrderHeader.Id,
					Price = cart.Price,
					Count = cart.Count,



				};
				_unitOfWork.OrderDetail.Add(OrderDetail);
				_unitOfWork.Save();

			}



			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				//it's a regluar coustomer account a we need to capture payment
				//Stripe Logic

				//var options = new Stripe.Checkout.SessionCreateOptions
				//{
				//	SuccessUrl = "https://example.com/success",
				//	LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
				//	{
				//		new Stripe.Checkout.SessionLineItemOptions
				//		{
				//			Price = "price_1MotwRLkdIwHu7ixYcPLm5uZ",
				//			Quantity = 2,
				//		},
				//	},
				//	Mode = "payment",
				//};
				//var service = new Stripe.Checkout.SessionService();
				//service.Create(options);

			}

			return RedirectToAction(nameof(OrderConfirmation), new { id = shoppingCartVM.OrderHeader.Id });
		}


		public IActionResult OrderConfirmation(int id)
		{
			HttpContext.Session.Clear();
			return View(id);
		}

		private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
		{
			if (shoppingCart.Count <= 50)
			{
				return shoppingCart.Product.Price;

			}
			else
			{
				if (shoppingCart.Count <= 100)
				{
					return shoppingCart.Product.Price50;
				}
				else
				{
					return shoppingCart.Product.Price100;


				}
			}

		}
	}
}
