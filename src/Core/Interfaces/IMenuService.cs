using Core.DTO;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IMenuService
    {
        void AddMenu(MenuDTO menuDTO);
        void DeleteMenu(int? id);
        void EditMenu(MenuDTO menuDTO);
        MenuDTO GetMenu(int? id);
        IEnumerable<MenuDTO> GetMenus(int? providerId);
        IEnumerable<MenuDTO> GetAllMenus();
        IEnumerable<MenuDishesDTO> GetMenuDishes(int? menuId);
        List<int> GetMenuIdDishes(int? menuId);
        void MakeMenu(int? menuId, List<int> newAddedDishes, List<int> allSelect);
        void DeleteDishInMenu(int? id);
        void Dispose();
    }
}
