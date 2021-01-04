using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class OrderDishes
    {
        public Guid Id { get; set; }
        [Required]
        public int Count { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int DishId { get; set; }
        public Dish Dish { get; set; }
    }
}
