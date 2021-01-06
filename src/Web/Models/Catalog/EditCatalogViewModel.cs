using System.ComponentModel.DataAnnotations;

namespace Web.Models.Catalog
{
    public class EditCatalogViewModel
    {
        public int ProviderId { get; set; }
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter info")]
        [Display(Name = "Info")]
        public string Info { get; set; }
    }
}
