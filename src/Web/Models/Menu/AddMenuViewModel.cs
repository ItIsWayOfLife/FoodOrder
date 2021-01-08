using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Menu
{
    public class AddMenuViewModel
    {
        public int ProviderId { get; set; }

        [Required(ErrorMessage = "Enter info")]
        [Display(Name = "Info")]
        public string Info { get; set; }

        [Required(ErrorMessage = "Enter date")]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
    }
}
