using Core.Entities;
using Core.Interfaces;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class CartDishesRepository : IRepository<CartDishes>
    {
        private readonly ApplicationContext _applicationContext;

        public CartDishesRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void Create(CartDishes item)
        {
            _applicationContext.CartDishes.Add(item);
        }

        public void Delete(int id)
        {
            CartDishes cartDishes = _applicationContext.CartDishes.Find(id);
            if (cartDishes != null)
            {
                _applicationContext.CartDishes.Remove(cartDishes);
            }
        }

        public IEnumerable<CartDishes> Find(Func<CartDishes, bool> predicate)
        {
            return _applicationContext.CartDishes.Include(c=>c.Cart).Include(d=>d.Dish).Where(predicate).ToList();
        }

        public CartDishes Get(int id)
        {
            return _applicationContext.CartDishes.Find(id);
        }

        public IEnumerable<CartDishes> GetAll()
        {
            return _applicationContext.CartDishes.Include(c => c.Cart).Include(d => d.Dish);
        }

        public void Update(CartDishes item)
        {
            _applicationContext.Entry(item).State = EntityState.Modified;
        }
    }
}
