using System.Collections.Generic;

namespace API.Models.Menu
{
    public class MakeMenuModel
    {
        public int MenuId { get; set; }
        public List<int> NewAddedDishes { get; set; }
        public List<int> AllSelect { get; set; }
    }
}
