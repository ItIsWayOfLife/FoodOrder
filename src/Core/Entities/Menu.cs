using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Menu 
    {
        public int Id { get; set; }

        [Required]
        public string Info { get; set; }

        [Required]
        public DateTime Date { get; set; }
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }
        public List<MenuDishes> MenuDishes { get; set; }

        public Menu()
        {
            MenuDishes = new List<MenuDishes>();
        }
    }
}
