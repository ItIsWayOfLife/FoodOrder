using AutoMapper;
using Core.Constants;
using Core.DTO;
using Core.Interfaces;
using System.Collections.Generic;
using Web.Interfaces;
using Web.Models.Provider;

namespace Web.Helper
{
    public class ProviderHelper : IProviderHelper
    {
        private readonly IProviderService _providerService;

        private readonly string _path;

        public ProviderHelper(IProviderService providerService)
        {
            _providerService = providerService;
            _path = PathConstants.PATH_PROVIDER;
        }

        public IEnumerable<ProviderViewModel> GetProviders()
        {
            IEnumerable<ProviderDTO> providersDTOs = _providerService.GetProviders();
           
            return ConvertProvidersDtoToView(providersDTOs);
        }

        public IEnumerable<ProviderViewModel> GetProvidersFavorite()
        {
            IEnumerable<ProviderDTO> providersDTOs = _providerService.GetFavoriteProviders();

            return ConvertProvidersDtoToView(providersDTOs);
        }

        public List<string> GetSearchSelection(bool isAdmin)
        {
            List<string> searchSelection = new List<string>() { "SearchBy" };

            if (isAdmin)
            {
                searchSelection.Add("Id");
            }

            searchSelection.AddRange(new string[] { "Name", "Email", "TimeWorkWith", "TimeWorkTo", "IsActive", "Inactive" });

            return searchSelection;
        }

        private IEnumerable<ProviderViewModel> ConvertProvidersDtoToView(IEnumerable<ProviderDTO> providersDTOs)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ProviderDTO, ProviderViewModel>()).CreateMapper();
            var providers = mapper.Map<IEnumerable<ProviderDTO>, List<ProviderViewModel>>(providersDTOs);

            foreach (var p in providers)
            {
                p.Path = _path + p.Path;
            }

            return providers;
        }
    }
}
