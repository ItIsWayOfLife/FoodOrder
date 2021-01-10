using System.ComponentModel.DataAnnotations;

namespace Web.Models.Dish
{
    public class EditDithViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter info")]
        [Display(Name = "Info")]
        public string Info { get; set; }

        [Required(ErrorMessage = "Enter weight")]
        [Display(Name = "Weight")]
        public double Weight { get; set; }

        [Required(ErrorMessage = "Enter price")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        public string Path { get; set; }
        public int CatalogId { get; set; }
    }
}
