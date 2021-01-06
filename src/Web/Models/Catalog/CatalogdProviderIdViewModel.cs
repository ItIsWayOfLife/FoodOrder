using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Web.Models.Catalog
{
    public class CatalogdProviderIdViewModel
    {
        public int ProviderId { get; set; }
        public int? MenuId { get; set; }
        public List<CatalogViewModel> Catalogs { get; set; }
        public SelectList SearchSelection { get; set; }
        public string SeacrhString { get; set; }
        public string SearchSelectionString { get; set; }
    }
}
