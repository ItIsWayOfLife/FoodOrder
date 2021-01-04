using Core.Entities;
using Core.Interfaces;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    class MenuRepository : IRepository<Menu>
    {
        private readonly ApplicationContext _applicationContext;

        public MenuRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void Create(Menu item)
        {
            _applicationContext.Menus.Add(item);
        }

        public void Delete(int id)
        {
            Menu menu = _applicationContext.Menus.Find(id);
            if (menu != null)
            {
                _applicationContext.Menus.Remove(menu);
            }
        }

        public IEnumerable<Menu> Find(Func<Menu, bool> predicate)
        {
            return _applicationContext.Menus.Include(p=>p.Provider).ToList();
        }

        public Menu Get(int id)
        {
            return _applicationContext.Menus.Find(id);
        }

        public IEnumerable<Menu> GetAll()
        {
            return _applicationContext.Menus.Include(p=>p.Provider).ToList();
        }

        public void Update(Menu item)
        {
            _applicationContext.Entry(item).State = EntityState.Modified;
        }
    }
}

