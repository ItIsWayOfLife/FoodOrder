using System.ComponentModel.DataAnnotations;

namespace Web.Models.Users
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Enter Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter First Name")]
        [Display(Name = "First Name")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Enter Second Name")]
        [Display(Name = "Second Name")]
        public string Lastname { get; set; }

        [Display(Name = "Middle Name")]
        public string Patronomic { get; set; }
    }
}
