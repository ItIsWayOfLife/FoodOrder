
namespace Web.Models.Dish
{
    public class DishViewModel
    {
        public int Id { get; set; }
        public int CatalogId { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public double Weight { get; set; }
        public decimal Price { get; set; }
        public string Path { get; set; }
        public bool AddMenu { get; set; }
    }
}
