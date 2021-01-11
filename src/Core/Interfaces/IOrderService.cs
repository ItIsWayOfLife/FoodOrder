using Core.DTO;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IOrderService
    {
        OrderDTO Create(string applicationUserId);
        IEnumerable<OrderDTO> GetOrders(string applicationUserId);
        IEnumerable<OrderDishesDTO> GetOrderDishes(string applicationUserId, int? orderId);
    }
}
