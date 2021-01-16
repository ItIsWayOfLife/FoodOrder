using API.Interfaces;
using API.Models.Cart;
using AutoMapper;
using Core.Constants;
using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class CartHelper : ICartHelper
    {
        public IEnumerable<CartDishesModel> ConvertCartDishesDTOsToCartDishesModel(IEnumerable<CartDishesDTO> cartDishesDTOs)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<CartDishesDTO, CartDishesModel>()).CreateMapper();
            var cartDishes = mapper.Map<IEnumerable<CartDishesDTO>, List<CartDishesModel>>(cartDishesDTOs);

            foreach (var cD in cartDishes)
            {
                cD.Path = PathAPIConstants.API_URL + PathAPIConstants.API_PATH_FILES + cD.Path;
            }

            return cartDishes;
        }
    }
}
