using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Web.Models.Menu
{
    public class MenuAndProviderIdViewModel
    {
        public int ProviderId { get; set; }
        public List<MenuViewModel> Menus { get; set; }
        public SelectList SearchSelection { get; set; }
        public string SeacrhString { get; set; }
        public string SearchSelectionString { get; set; }
    }
}
