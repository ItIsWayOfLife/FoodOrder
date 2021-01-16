using API.Models.Order;
using Core.DTO;
using System.Collections.Generic;

namespace API.Interfaces
{
    public interface IOrderHelper
    {
        IEnumerable<OrderDishesModel> ConvertOrderDishesDTOToOrderDishesModels(IEnumerable<OrderDishesDTO> orderDishesDTOs);
    }
}
