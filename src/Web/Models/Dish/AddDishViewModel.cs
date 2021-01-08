using System.ComponentModel.DataAnnotations;

namespace Web.Models.Dish
{
    public class AddDishViewModel
    {
        public int CatalogId { get; set; }

        [Required(ErrorMessage = "NameNotSpecified")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "InfoNotSpecified")]
        [Display(Name = "Info")]
        public string Info { get; set; }

        [Required(ErrorMessage = "WeightNotSpecified")]
        [Display(Name = "Weight")]
        public double Weight { get; set; }

        [Required(ErrorMessage = "PriceNotSpecified")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }
        public string Path { get; set; }
    }
}
