using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Provider
{
    public class AddProviderViewModel
    {
        [Required(ErrorMessage = "Enter name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter time work with")]
        [Display(Name = "TimeWorkWith")]
        public DateTime TimeWorkWith { get; set; }

        [Required(ErrorMessage = "Enter time work to")]
        [Display(Name = "TimeWorkTo")]
        public DateTime TimeWorkTo { get; set; }
        public bool IsActive { get; set; }
        public string Path { get; set; }
        public string WorkingDays { get; set; }
        public bool IsFavorite { get; set; }
        public string Info { get; set; }
    }
}
