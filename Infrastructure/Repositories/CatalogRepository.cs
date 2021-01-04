using Core.Entities;
using Core.Interfaces;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories 
{
    public class CatalogRepository : IRepository<Catalog>
    {
        private readonly ApplicationContext _applicationContext;

        public CatalogRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void Create(Catalog item)
        {
            _applicationContext.Catalogs.Add(item);
        }

        public void Delete(int id)
        {
            Catalog menu = _applicationContext.Catalogs.Find(id);
            if (menu != null)
            {
                _applicationContext.Catalogs.Remove(menu);
            }
        }

        public IEnumerable<Catalog> Find(Func<Catalog, bool> predicate)
        {
            return _applicationContext.Catalogs.Include(p => p.Provider).Where(predicate).ToList();
        }

        public Catalog Get(int id)
        {
            return _applicationContext.Catalogs.Find(id);
        }

        public IEnumerable<Catalog> GetAll()
        {
            return _applicationContext.Catalogs.Include(p => p.Provider);
        }

        public void Update(Catalog item)
        {
            _applicationContext.Entry(item).State = EntityState.Modified;
        }
    }
}
