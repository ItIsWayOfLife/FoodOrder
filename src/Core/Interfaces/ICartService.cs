using Core.DTO;
using Core.Entities;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface ICartService
    {
        Cart Create(string applicationUserId);

        CartDTO GetCart(string applicationUserId);

        IEnumerable<CartDishesDTO> GetCartDishes(string applicationUserId);

        void DeleteCartDish(int? id, string currentUserId);

        void AddDishToCart(int? dishId, string applicationUserId);

        void AllDeleteDishesToCart(string applicationUserId);

        void UpdateCountDishInCart(string applicationUserId, int? dishCartId, int count);

        decimal FullPriceCart(string applicationUserId);
    }
}
