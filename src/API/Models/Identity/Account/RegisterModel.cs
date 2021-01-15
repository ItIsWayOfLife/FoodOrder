using System.ComponentModel.DataAnnotations;

namespace API.Models.Identity.Account
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Enter email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter First Name")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Enter Second Name")]
        public string Lastname { get; set; }

        public string Patronymic { get; set; }

        [Required(ErrorMessage = "Enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm the password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}
