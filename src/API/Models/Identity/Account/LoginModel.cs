using System.ComponentModel.DataAnnotations;

namespace API.Models.Identity.Account
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Enter email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
