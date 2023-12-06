using System.ComponentModel.DataAnnotations;

namespace kursach_4._12._23.Models
{
    public class ProductModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
    }
}
