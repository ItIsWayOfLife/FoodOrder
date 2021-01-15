using System.ComponentModel.DataAnnotations;

namespace API.Models.Identity.Account
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Enter new password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Enter old password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
    }
}
