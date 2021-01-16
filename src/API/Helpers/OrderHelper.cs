using API.Interfaces;
using API.Models.Order;
using AutoMapper;
using Core.Constants;
using Core.DTO;
using System.Collections.Generic;

namespace API.Helpers
{
    public class OrderHelper : IOrderHelper
    {
        public IEnumerable<OrderDishesModel> ConvertOrderDishesDTOToOrderDishesModels(IEnumerable<OrderDishesDTO> orderDishesDTOs)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<OrderDishesDTO, OrderDishesModel>()).CreateMapper();
            var orderDishesModels = mapper.Map<IEnumerable<OrderDishesDTO>, List<OrderDishesModel>>(orderDishesDTOs);

            foreach (var orderDishesModel in orderDishesModels)
            {
                orderDishesModel.Path = PathAPIConstants.API_URL + PathAPIConstants.API_PATH_FILES + orderDishesModel.Path;
            }

            return orderDishesModels;
        }
    }
}
