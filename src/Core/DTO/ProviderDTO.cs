using System;

namespace Core.DTO
{
    public class ProviderDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime TimeWorkWith { get; set; }
        public DateTime TimeWorkTo { get; set; }
        public bool IsActive { get; set; }
        public string Path { get; set; }
        public string WorkingDays { get; set; }
        public bool IsFavorite { get; set; }
        public string Info { get; set; }
    }
}
