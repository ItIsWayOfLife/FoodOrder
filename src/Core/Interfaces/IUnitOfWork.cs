using Core.Entities;
using System;

namespace Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Provider> Provider { get; }
        IRepository<Catalog> Catalog { get; }
        IRepository<Dish> Dish { get; }
        IRepository<CartDishes> CartDishes { get; }
        IRepository<Cart> Cart { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderDishes> OrderDishes { get; }
        IRepository<Menu> Menu { get; }
        IRepository<MenuDishes> MenuDishes { get; }

        void Save();
    }
}
