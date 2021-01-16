using System.ComponentModel.DataAnnotations;

namespace API.Models.Menu
{
    public class MenuModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter info")]
        public string Info { get; set; }

        [Required(ErrorMessage = "Enter date")]
        public string Date { get; set; }
        public int ProviderId { get; set; }
    }
}
