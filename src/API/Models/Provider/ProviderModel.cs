
namespace API.Models.Provider
{
    public class ProviderModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string TimeWorkWith { get; set; }
        public string TimeWorkTo { get; set; }
        public bool IsActive { get; set; }
        public bool IsFavorite { get; set; }
        public string Path { get; set; }
        public string WorkingDays { get; set; }
        public string Info { get; set; }
    }
}
