using System.ComponentModel.DataAnnotations;

namespace Web.Models.Catalog
{
    public class AddCatalogViewModel
    {
        public int ProviderId { get; set; }

        [Required(ErrorMessage = "Enter name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter info")]
        [Display(Name = "Info")]
        public string Info { get; set; }
    }
}
