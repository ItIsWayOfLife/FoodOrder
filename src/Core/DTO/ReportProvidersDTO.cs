
namespace Core.DTO
{
    public class ReportProvidersDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal FullPrice { get; set; }
        public int CountOrderDishes { get; set; }
        public int CountOrder { get; set; }
    }
}
