
namespace Core.DTO
{
    public class ReportUsersDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string LFP { get; set; }
        public decimal FullPrice { get; set; }
        public int CountOrderDishes { get; set; }
        public int CountOrder { get; set; }
    }
}
