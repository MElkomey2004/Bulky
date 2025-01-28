using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class DeleteModel : PageModel
    {

		private readonly ApplicationDbContext _db;

		[BindProperty]
		public Category Category { get; set; }
		public DeleteModel(ApplicationDbContext db)
		{
			_db = db;

		}
        // Ensure you're getting the category in the OnGet method
        public void OnGet(int? id)
        {
            //if (id == null || id == 0)
            //{
            //    return NotFound();
            //}

            //Category = _db.Categories.Find(id);

            //if (Category == null)
            //{
            //    return NotFound();
            //}

            //return Page();

            if(id != null && id != 0)
            {
                Category = _db.Categories.Find(id);
            } 
        }

        public IActionResult OnPost()
        {
            var category = _db.Categories.Find(Category.Id);


            _db.Categories.Remove(category);
            _db.SaveChanges();
            TempData["Success"] = "Category Deleted Successfuly";


            return RedirectToPage("Index");
        }
    }
}
