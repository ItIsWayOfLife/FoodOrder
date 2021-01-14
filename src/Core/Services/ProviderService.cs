using Core.DTO;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace Core.Services
{
    public class ProviderService : IProviderService
    {
        private IUnitOfWork Database { get; set; }

        public ProviderService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public IEnumerable<ProviderDTO> GetProviders()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Provider, ProviderDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Provider>, List<ProviderDTO>>(Database.Provider.GetAll());
        }

        public IEnumerable<ProviderDTO> GetFavoriteProviders()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Provider, ProviderDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Provider>, List<ProviderDTO>>(Database.Provider.GetAll()).Where(f => f.IsFavorite == true);
        }

        public void AddProvider(ProviderDTO providerDTO)
        {
            if (providerDTO.Name == null)
                throw new ValidationException("Name not set", string.Empty);

            if (providerDTO.Email == null)
                throw new ValidationException("Email not set", string.Empty);

            if (providerDTO.TimeWorkWith == null)
                throw new ValidationException("TimeWorkWith not set", string.Empty);

            if (providerDTO.TimeWorkTo == null)
                throw new ValidationException("TimeWorkTo not set", string.Empty);

            Provider provider = new Provider()
            {
                Email = providerDTO.Email,
                Info = providerDTO.Info,
                IsActive = providerDTO.IsActive,
                IsFavorite = providerDTO.IsFavorite,
                Name = providerDTO.Name,
                Path = providerDTO.Path,
                TimeWorkTo = providerDTO.TimeWorkTo,
                TimeWorkWith = providerDTO.TimeWorkWith,
                WorkingDays = providerDTO.WorkingDays
            };

            Database.Provider.Create(provider);
            Database.Save();
        }

        public void DeleteProvider(int? id)
        {
            if (id == null)
                throw new ValidationException("Provider id not set", string.Empty);

            var provider = Database.Provider.Get(id.Value);

            if (provider == null)
                throw new ValidationException("Provider not found", string.Empty);

            var dishesInMenu = Database.MenuDishes.GetAll().Where(p => p.Menu.ProviderId == id.Value);

            foreach (var dishInMenu in dishesInMenu)
            {
                Database.MenuDishes.Delete(dishInMenu.Id);
            }

            Database.Provider.Delete(id.Value);
            Database.Save();
        }

        public void EditProvider(ProviderDTO providerDTO)
        {
            if (providerDTO.Name == null)
                throw new ValidationException("Name not set", string.Empty);

            if (providerDTO.Email == null)
                throw new ValidationException("Email not set", string.Empty);

            if (providerDTO.TimeWorkWith == null)
                throw new ValidationException("TimeWorkWith not set", string.Empty);

            if (providerDTO.TimeWorkTo == null)
                throw new ValidationException("TimeWorkTo not set", string.Empty);

            Provider provider = Database.Provider.Get(providerDTO.Id);

            if (provider == null)
                throw new ValidationException("Provider not found", string.Empty);
            
            provider.Email = providerDTO.Email;
            provider.Info = providerDTO.Info;
            provider.IsActive = providerDTO.IsActive;
            provider.IsFavorite = providerDTO.IsFavorite;
            provider.Name = providerDTO.Name;
            provider.Path = providerDTO.Path;
            provider.TimeWorkTo = providerDTO.TimeWorkTo;
            provider.TimeWorkWith = providerDTO.TimeWorkWith;
            provider.WorkingDays = providerDTO.WorkingDays;

            Database.Provider.Update(provider);
            Database.Save();
        }

        public ProviderDTO GetProvider(int? id)
        {
            if (id == null)
                throw new ValidationException("Provider id not set", string.Empty);

            var provider = Database.Provider.Get(id.Value);

            if (provider == null)
                throw new ValidationException("Provider not found", string.Empty);

            ProviderDTO providerDTO = new ProviderDTO()
            {
                Id = provider.Id,
                WorkingDays = provider.WorkingDays,
                Email = provider.Email,
                Info = provider.Info,
                IsActive = provider.IsActive,
                IsFavorite = provider.IsFavorite,
                Name = provider.Name,
                Path = provider.Path,
                TimeWorkTo = provider.TimeWorkTo,
                TimeWorkWith = provider.TimeWorkWith
            };

            return providerDTO;
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }

}
