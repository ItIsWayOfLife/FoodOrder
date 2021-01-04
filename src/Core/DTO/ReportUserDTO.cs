using System;

namespace Core.DTO
{
    public class ReportUserDTO
    {
        public int Id { get; set; }
        public DateTime DateOrder { get; set; }
        public decimal FullPrice { get; set; }
        public int CountOrderDishes { get; set; }
    }
}
