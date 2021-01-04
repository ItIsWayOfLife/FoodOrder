using Core.Entities;
using Core.Interfaces;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class ProviderRepository : IRepository<Provider>
    {
        private readonly ApplicationContext _applicationContext;

        public ProviderRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void Create(Provider item)
        {
            _applicationContext.Providers.Add(item);
        }

        public void Delete(int id)
        {
            Provider provider = _applicationContext.Providers.Find(id);
            if (provider != null)
            {
                _applicationContext.Providers.Remove(provider);
            }
        }

        public IEnumerable<Provider> Find(Func<Provider, bool> predicate)
        {
            return _applicationContext.Providers.Where(predicate).ToList();
        }

        public Provider Get(int id)
        {
            return _applicationContext.Providers.Find(id);
        }

        public IEnumerable<Provider> GetAll()
        {
            return _applicationContext.Providers;
        }

        public void Update(Provider item)
        {
            _applicationContext.Entry(item).State = EntityState.Modified;
        }
    }
}
