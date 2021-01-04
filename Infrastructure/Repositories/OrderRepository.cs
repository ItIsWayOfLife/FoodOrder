using Core.Entities;
using Core.Interfaces;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class OrderRepository : IRepository<Order>
    {
        private readonly ApplicationContext _applicationContext;

        public OrderRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void Create(Order item)
        {
            _applicationContext.Orders.Add(item);
        }

        public void Delete(int id)
        {
            Order order = _applicationContext.Orders.Find(id);
            if (order != null)
            {
                _applicationContext.Orders.Remove(order);
            }
        }

        public IEnumerable<Order> Find(Func<Order, bool> predicate)
        {
            return _applicationContext.Orders.Where(predicate).ToList();
        }

        public Order Get(int id)
        {
            return _applicationContext.Orders.Find(id);
        }

        public IEnumerable<Order> GetAll()
        {
            return _applicationContext.Orders;
        }

        public void Update(Order item)
        {
            _applicationContext.Entry(item).State = EntityState.Modified;
        }
    }
}
