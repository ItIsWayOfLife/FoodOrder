using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public DateTime DateOrder { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
    }
}
