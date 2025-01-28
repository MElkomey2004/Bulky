using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public  class CompanyRepsitory : Repository<Company>, ICompanyRepository
    {

        private readonly ApplicationDbContext _db;
        private readonly ICompanyRepository _companyRepository;

        public CompanyRepsitory(ApplicationDbContext db ) :base(db) { 
        
            _db = db;
         
        
        
        }


       public void Update(Company obj)
        {
            _db.Companies.Update(obj);    
        }

    }
}
