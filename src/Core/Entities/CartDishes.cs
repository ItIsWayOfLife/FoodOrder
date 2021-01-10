using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class CartDishes
    {
        public int Id { get; set; }

        [Required]
        public int Count { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public int DishId { get; set; }
        public Dish Dish { get; set; }
    }
}
