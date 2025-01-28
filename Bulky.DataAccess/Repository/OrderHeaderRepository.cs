using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
	public  class OrderHeaderRepository : Repository<OrderHeader> ,  IOrderHeaderRepository
	{

		private ApplicationDbContext _db;

	
		public OrderHeaderRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;

			
		}

		public void Update(OrderHeader obj)
		{
			_db.OrderHeaders.Update(obj);
		}

		void IOrderHeaderRepository.Update(OrderHeader obj)
		{
			//throw new NotImplementedException();
		}

		void UpdateStatus(int id, string OrderStatus, string? PaymentStatus)
		{		
			var orderFromDb = _db.OrderHeaders.FirstOrDefault( u => u.Id == id);
			//if (orderFromDb != null)
			//{
			//	orderFromDb.OrderStatus = OrderStatus;
			//	if (!String.IsNullOrEmpty(PaymentStatus))
			//	{
			//		orderFromDb.PaymentStatus = PaymentStatus;
			//	}
			//}
		}

		void IOrderHeaderRepository.UpdateStatus(int id, string OrderStatus, string? PaymentStatus)
		{
			//throw new NotImplementedException();
		}

		void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
		{
			//var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);

			//if (!string.IsNullOrEmpty(sessionId))
			//{
			//	//orderFromDb.SessionId = sessionId;
			//}

			//if (!string.IsNullOrEmpty(paymentIntentId))
			//{
			//	orderFromDb.PaymentIntentId = paymentIntentId;
			//	orderFromDb.PaymentDate = DateTime.Now;
			//}
		}

		void IOrderHeaderRepository.UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
		{
			//throw new NotImplementedException();
		}
	}
}
