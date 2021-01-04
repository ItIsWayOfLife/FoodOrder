using System;

namespace Core.Entities
{
    public class MenuDishes
    {
        public Guid Id { get; set; }
        public int? MenuId { get; set; }
        public Menu Menu { get; set; }
        public int? DishId { get; set; }
        public Dish Dish { get; set; }
    }
}
