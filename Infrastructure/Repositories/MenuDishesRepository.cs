using Core.Entities;
using Core.Interfaces;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    class MenuDishesRepository : IRepository<MenuDishes>
    {
        private readonly ApplicationContext _applicationContext;

        public MenuDishesRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void Create(MenuDishes item)
        {
            _applicationContext.MenuDishes.Add(item);
        }

        public void Delete(int id)
        {
            MenuDishes menuDish = _applicationContext.MenuDishes.Find(id);
            if (menuDish != null)
            {
                _applicationContext.MenuDishes.Remove(menuDish);
            }
        }

        public IEnumerable<MenuDishes> Find(Func<MenuDishes, bool> predicate)
        {
            return _applicationContext.MenuDishes.Include(p => p.Dish).Include(p => p.Menu).ToList();
        }

        public MenuDishes Get(int id)
        {
            return _applicationContext.MenuDishes.Find(id);
        }

        public IEnumerable<MenuDishes> GetAll()
        {
            return _applicationContext.MenuDishes.Include(p => p.Menu).Include(p => p.Dish).ToList();
        }

        public void Update(MenuDishes item)
        {
            _applicationContext.Entry(item).State = EntityState.Modified;
        }
    }
}
