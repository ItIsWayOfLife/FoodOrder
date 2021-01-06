using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Models.Provider
{
    public class ProviderListViewModel
    {
        public SelectList SearchSelection { get; set; }
        public ListProviderViewModel ListProviders { get; set; }
        public string SeacrhString { get; set; }
        public string SearchSelectionString { get; set; }
    }
}
