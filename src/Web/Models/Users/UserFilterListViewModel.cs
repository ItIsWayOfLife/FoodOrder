using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Users
{
    public class UserFilterListViewModel
    {
        public SelectList SearchSelection { get; set; }
        public ListUserViewModel ListUsers { get; set; }

        [Display(Name = "Search")]
        public string SeacrhString { get; set; }

        [Display(Name = "Search selection")]
        public string SearchSelectionString { get; set; }
    }
}
