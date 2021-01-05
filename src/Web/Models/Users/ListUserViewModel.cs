using System.Collections.Generic;

namespace Web.Models.Users
{
    public class ListUserViewModel
    {
        public IEnumerable<UserViewModel> Users { get; set; }
    }
}
