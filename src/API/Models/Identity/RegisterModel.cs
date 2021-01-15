
namespace API.Models.Identity
{
    public class RegisterModel
    {
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Patronymic { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
    }
}
