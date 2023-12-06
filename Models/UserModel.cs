using System.ComponentModel.DataAnnotations;

namespace kursach_4._12._23.Models
{
    public class UserModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
