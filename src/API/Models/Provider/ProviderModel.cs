
using System.ComponentModel.DataAnnotations;

namespace API.Models.Provider
{
    public class ProviderModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter time work with")]
        public string TimeWorkWith { get; set; }

        [Required(ErrorMessage = "Enter time work to")]
        public string TimeWorkTo { get; set; }
        public bool IsActive { get; set; }
        public bool IsFavorite { get; set; }
        public string Path { get; set; }
        public string WorkingDays { get; set; }
        public string Info { get; set; }
    }
}
