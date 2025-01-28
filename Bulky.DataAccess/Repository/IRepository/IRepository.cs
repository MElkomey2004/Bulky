using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
	public interface IRepository<T>  where T :   class
	{

		IEnumerable<T> GetAll(Expression<Func<T, bool>>? Filter =null , string? IncludeProperites = null);	

		T Get(Expression<Func<T, bool>> Filter , string? IncludeProperites = null , bool tracked = false);
		
		void Add(T entity);	
		void Remove(T entity);	

		void RemoveRange(IEnumerable<T> entity);
	}
}
