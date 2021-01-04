using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Dish
    {
        public int Id { get; set; }
        public int CatalogId { get; set; }
        public Catalog Сatalog { get; set; }
        public string Name { get; set; }
        [Required]
        public string Info { get; set; }
        [Required]
        public double Weight { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string Path { get; set; }
        public List<MenuDishes> MenuDishes { get; set; }

        public Dish()
        {
            MenuDishes = new List<MenuDishes>();
        }
    }
}
