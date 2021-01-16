using System.ComponentModel.DataAnnotations;

namespace API.Models.Dish
{
    public class DishModel
    {
        public int Id { get; set; }
        public int CatalogId { get; set; }

        [Required(ErrorMessage = "Enter name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter name")]
        public string Info { get; set; }

        [Required(ErrorMessage = "Enter weight")]
        public double Weight { get; set; }

        [Required(ErrorMessage = "Enter price")]
        public decimal Price { get; set; }
        public string Path { get; set; }
        public bool AddMenu { get; set; }
    }
}
