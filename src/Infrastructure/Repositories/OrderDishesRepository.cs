using Core.Entities;
using Core.Interfaces;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class OrderDishesRepository : IRepository<OrderDishes>
    {
        private readonly ApplicationContext _applicationContext;

        public OrderDishesRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void Create(OrderDishes item)
        {
            _applicationContext.OrderDishes.Add(item);
        }

        public void Delete(int id)
        {
            OrderDishes orderDishes = _applicationContext.OrderDishes.Find(id);
            if (orderDishes != null)
            {
                _applicationContext.OrderDishes.Remove(orderDishes);
            }
        }

        public IEnumerable<OrderDishes> Find(Func<OrderDishes, bool> predicate)
        {
            return _applicationContext.OrderDishes.Include(o => o.Order).Include(d => d.Dish).Where(predicate).ToList();
        }

        public OrderDishes Get(int id)
        {
            return _applicationContext.OrderDishes.Find(id);
        }

        public IEnumerable<OrderDishes> GetAll()
        {
            return _applicationContext.OrderDishes.Include(o => o.Order).Include(d => d.Dish);
        }

        public void Update(OrderDishes item)
        {
            _applicationContext.Entry(item).State = EntityState.Modified;
        }
    }
}
