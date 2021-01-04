using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class Menu 
    {
        public int Id { get; set; }
        public string Info { get; set; }
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
