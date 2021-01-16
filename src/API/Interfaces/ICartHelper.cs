using API.Models.Cart;
using Core.DTO;
using System.Collections.Generic;

namespace API.Interfaces
{
   public  interface ICartHelper
    {
        IEnumerable<CartDishesModel> ConvertCartDishesDTOsToCartDishesModel(IEnumerable<CartDishesDTO> cartDishesDTOs);
    }
}
