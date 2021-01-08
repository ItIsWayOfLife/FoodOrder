using Core.DTO;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IDishService
    {
        void AddDish(DishDTO dishDTO);
        void DeleteDish(int? id);
        void EditDish(DishDTO dishDTO);
        DishDTO GetDish(int? id);
        IEnumerable<DishDTO> GetDishesForMenu(int? catalogId, List<int> addedDishes);
        IEnumerable<DishDTO> GetDishes(int? catalogId);
        IEnumerable<DishDTO> GetAllDishes();

        void Dispose();
    }
}
