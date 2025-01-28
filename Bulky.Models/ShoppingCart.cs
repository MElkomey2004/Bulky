using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class ShoppingCart
    {
	
		public int Id { get; set; } // This should be an auto-incremented column in the database
		public int ProductId { get; set; }
		[ForeignKey("ProductId")]
		[ValidateNever]
		public Product Product { get; set; }
		public string ApplicationUserId { get; set; }
		
		[ForeignKey("ApplicationUserId")]
		[ValidateNever]	
		public ApplicationUser ApplicationUser { get; set; }

		[Range(1, 1000, ErrorMessage = "Please Enter a value Between 1 and 1000")]
		public int Count { get; set; }



		[NotMapped] // Not Created a column to Database and not when add these not needed to updated migration
		public double Price { get; set; }

    }
}
