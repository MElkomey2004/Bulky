﻿using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
       public DbInitializer(UserManager<IdentityUser> userManager , RoleManager<IdentityRole> roleManager, ApplicationDbContext db) 
        { 
            
            _userManger = userManager;
            _roleManager = roleManager;
            _db = db;

        
        
        }
        public  void Initialize()
        {
            //migration if they are not appllied


            try { 
                
                if(_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch(Exception ex) {

                //create roles if they are not created 

                if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                }
                //if roles are not created , then we will create admin user as well.

                _userManger.CreateAsync(new ApplicationUser
                {
                    UserName = "Moahmed@gmail.com",
                    Email = "elkomey2004@gmail.com",
                    Name = "Mohamed Wael",
                    PhoneNumber = "983743",
                    StreetAdress = "348ydkfd eiru",
                    State = "IL",
                    PostalCode = "34380",
                    City = "Chicago"
                }, "Admin123*").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "elkomey2004@gmail.com");
                _userManger.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

            }

            

            return;

        }
    }
}
