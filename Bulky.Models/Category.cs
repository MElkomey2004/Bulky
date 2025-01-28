using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models
{
    public class Category
    {

        public int Id { get; set; }




        public string Name { get; set; }

        [Display(Name = "Display Order")]

        [Range(1, 100, ErrorMessage = "Display Order Must be between 1 to 3")]
        public int DisplayOrder { get; set; }

    }
}
