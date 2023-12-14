using System.ComponentModel.DataAnnotations;

namespace kursach_4._12._23.Models
{
    public class CartModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Продукт обязателен")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Пользователь обязателен")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Количество обязательно")]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        public int Count { get; set; }
    }
}

