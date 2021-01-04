using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Catalog 
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Info { get; set; }
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }

        public ICollection<Dish> Dishes { get; set; }
    }
}
