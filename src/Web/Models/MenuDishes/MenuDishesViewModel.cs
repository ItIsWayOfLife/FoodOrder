
namespace Web.Models.MenuDishes
{
    public class MenuDishesViewModel
    {
        public int Id { get; set; }
        public int? MenuId { get; set; }
        public int? DishId { get; set; }
        public int CatalogId { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public double Weight { get; set; }
        public decimal Price { get; set; }
        public string Path { get; set; }
    }
}
