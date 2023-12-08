using System.ComponentModel.DataAnnotations;

namespace kursach_4._12._23.Models
{
    public class ProductModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, float.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public string Price { get; set; }

        [Required(ErrorMessage = "Count is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Count must be greater than 0")]
        public int Count { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }
    }

}
