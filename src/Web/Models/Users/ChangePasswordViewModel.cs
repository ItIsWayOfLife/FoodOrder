using System.ComponentModel.DataAnnotations;

namespace Web.Models.Users
{
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is incorrect")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Password is incorrect")]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }
    }
}
