using System;

namespace Web.Models.Menu
{
    public class MenuViewModel
    {
        public int Id { get; set; }
        public string Info { get; set; }
        public DateTime Date { get; set; }
        public int ProviderId { get; set; }
        public string ProviderName { get; set; }

    }
}
