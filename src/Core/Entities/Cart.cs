using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        
        [Required]
        public string ApplicationUserId { get; set; }
    }
}
