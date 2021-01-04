using Core.Entities;
using Core.Interfaces;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class CartRepository : IRepository<Cart>
    {
        private readonly ApplicationContext _applicationContext;

        public CartRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void Create(Cart item)
        {
            _applicationContext.Carts.Add(item);
        }

        public void Delete(int id)
        {
            Cart cart = _applicationContext.Carts.Find(id);
            if (cart != null)
            {
                _applicationContext.Carts.Remove(cart);
            }
        }

        public IEnumerable<Cart> Find(Func<Cart, bool> predicate)
        {
            return _applicationContext.Carts.Where(predicate).ToList();
        }

        public Cart Get(int id)
        {
            return _applicationContext.Carts.Find(id);
        }

        public IEnumerable<Cart> GetAll()
        {
            return _applicationContext.Carts;
        }

        public void Update(Cart item)
        {
            _applicationContext.Entry(item).State = EntityState.Modified;
        }
    }
}
