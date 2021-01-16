using API.Interfaces;
using API.Models.Dish;
using Core.Constants;
using Core.DTO;
using System.Collections.Generic;

namespace API.Helpers
{
    public class DishHelper: IDishHelper
    {
        private readonly string _path;

        public DishHelper()
        {
            _path = PathAPIConstants.API_URL + PathAPIConstants.API_PATH_FILES;
        }

        public DishDTO ConvertDishModelToDishDTO(DishModel model)
        {
            return new DishDTO()
            {
                Id = model.Id,
                Info = model.Info,
                CatalogId = model.CatalogId,
                Name = model.Name,
                Price = model.Price,
                Weight = model.Weight,
                Path = model.Path.Replace(_path, string.Empty)
            };
        }

        public DishModel ConvertDishDTOToDishModel(DishDTO dishDTO)
        {
            return new DishModel()
            {
                Id = dishDTO.Id,
                Info = dishDTO.Info,
                AddMenu = dishDTO.AddMenu,
                CatalogId = dishDTO.CatalogId,
                Name = dishDTO.Name,
                Path = _path + dishDTO.Path,
                Price = dishDTO.Price,
                Weight = dishDTO.Weight
            };
        }

        public IEnumerable<DishModel> ConvertDishDTOsToDishModels(IEnumerable<DishDTO> dishDTOs)
        {
            var dishModels = new List<DishModel>();

            foreach (var d in dishDTOs)
            {
                dishModels.Add(ConvertDishDTOToDishModel(d));
            }

            return dishModels;
        }
    }
}
