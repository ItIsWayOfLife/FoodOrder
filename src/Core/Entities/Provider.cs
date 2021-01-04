using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Provider
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public DateTime TimeWorkWith { get; set; }
        [Required]
        public DateTime TimeWorkTo { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public string Path { get; set; }
        public string WorkingDays { get; set; }
        [Required]
        public bool IsFavorite { get; set; }
        public string Info { get; set; }
        public ICollection<Catalog> Сatalogs { get; set; }
    }
}
