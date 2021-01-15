using System.ComponentModel.DataAnnotations;

namespace API.Models.Identity.Users
{
    public class UserModelChangePasword
    {
        [Required(ErrorMessage = "Enter id")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Enter new password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Enter old password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
    }
}
