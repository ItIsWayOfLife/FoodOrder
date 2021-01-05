using Core.Entities;
using Core.Interfaces;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class DishRepository : IRepository<Dish>
    {
        private readonly ApplicationContext _applicationContext;

        public DishRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void Create(Dish item)
        {
            _applicationContext.Dishes.Add(item);
        }

        public void Delete(int id)
        {
            Dish dish = _applicationContext.Dishes.Find(id);
            if (dish != null)
            {
                _applicationContext.Dishes.Remove(dish);
            }
        }

        public IEnumerable<Dish> Find(Func<Dish, bool> predicate)
        {
            return _applicationContext.Dishes.Include(c=>c.Сatalog).Where(predicate).ToList();
        }

        public Dish Get(int id)
        {
            return _applicationContext.Dishes.Find(id);
        }

        public IEnumerable<Dish> GetAll()
        {
            return _applicationContext.Dishes.Include(c=>c.Сatalog);
        }

        public void Update(Dish item)
        {
            _applicationContext.Entry(item).State = EntityState.Modified;
        }
    }
}
