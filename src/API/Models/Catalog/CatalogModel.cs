using System.ComponentModel.DataAnnotations;

namespace API.Models.Catalog
{
    public class CatalogModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter info")]
        public string Info { get; set; }
        public int ProviderId { get; set; }
    }
}
