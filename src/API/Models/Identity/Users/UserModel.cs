using System.ComponentModel.DataAnnotations;

namespace API.Models.Identity.Users
{
    public class UserModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Enter email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter First Name")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Enter Second Name")]
        public string Lastname { get; set; }

        public string Patronymic { get; set; }

        public string Password { get; set; }
    }
}
