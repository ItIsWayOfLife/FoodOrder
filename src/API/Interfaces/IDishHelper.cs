using API.Models.Dish;
using Core.DTO;
using System.Collections.Generic;

namespace API.Interfaces
{
   public interface IDishHelper
    {
        DishDTO ConvertDishModelToDishDTO(DishModel model);
        DishModel ConvertDishDTOToDishModel(DishDTO dishDTO);
        IEnumerable<DishModel> ConvertDishDTOsToDishModels(IEnumerable<DishDTO> dishDTOs);
    }
}
