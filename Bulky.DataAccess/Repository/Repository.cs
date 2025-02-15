﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace BulkyBook.DataAccess.Repository
{
	public class Repository <T> : IRepository<T> where T : class
	{
		private readonly ApplicationDbContext _db;
		internal DbSet<T> dbSet;
		
		public Repository(ApplicationDbContext db)
		{
			_db = db;

			this.dbSet = _db.Set<T>();
			//db.categories == dbset;

			//_db.Products.Include(u => u.Category).Include(u => u.CategoryId);

		}
		public void Add(T entity)
		{
			dbSet.Add(entity);
		}

		public T Get(Expression<Func<T, bool>> Filter , string? IncludeProperites = null, bool tracked = false)
		{
			IQueryable<T> query;
			if (tracked)
			{


				query = dbSet;

			}
			else {		

					query = dbSet.AsNoTracking();
				
			}



			query = dbSet.Where(Filter);

			if (!string.IsNullOrEmpty(IncludeProperites))
			{
				foreach (var includeProp in IncludeProperites.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProp);

				}
			}


			return query.FirstOrDefault();

		}

		public IEnumerable<T> GetAll(Expression<Func<T, bool>>? Filter, string? IncludeProperites = null)
		{
			IQueryable<T> query = dbSet;

			if(Filter != null)
			{
				 query = dbSet.Where(Filter);

			}


            if (!string.IsNullOrEmpty(IncludeProperites))
			{
				foreach(var includeProp in IncludeProperites.Split(new char[] {','} , StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProp);

				}
			}

			return query.ToList();

		}

		public void Remove(T entity)
		{
			dbSet.Remove(entity);
		}

		public void RemoveRange(IEnumerable<T> entity)
		{
			dbSet.RemoveRange(entity);
		}
	}
}
