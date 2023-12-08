using System.ComponentModel.DataAnnotations;

namespace kursach_4._12._23.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Name is required")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Invalid name format")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
