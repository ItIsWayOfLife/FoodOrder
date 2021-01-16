using API.Interfaces;
using API.Models.Dish;
using Core.Constants;
using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public DishModel ConvertDishDTOToDishModel(DishDTO dto)
        {
            return new DishModel()
            {
                Id = dto.Id,
                Info = dto.Info,
                AddMenu = dto.AddMenu,
                CatalogId = dto.CatalogId,
                Name = dto.Name,
                Path = _path + dto.Path,
                Price = dto.Price,
                Weight = dto.Weight
            };
        }

        public IEnumerable<DishModel> ConvertDishDTOTODishModel(IEnumerable<DishDTO> dishDTOs)
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
