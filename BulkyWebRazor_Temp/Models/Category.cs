
using System.ComponentModel.DataAnnotations;

namespace BulkyWebRazor_Temp.Models
{
    public class Category
    {

        public int Id { get; set; }



        public string Name { get; set; }

        [Display(Name = "Display Order")]

        [Range(1, 100, ErrorMessage = "Display Order Must be between 1 to 100")]
        public int DisplayOrder { get; set; }

    }
}
