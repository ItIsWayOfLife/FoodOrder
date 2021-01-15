using API.Interfaces;
using API.Models.Provider;
using Core.Constants;
using Core.DTO;
using System;

namespace API.Helpers
{
    public class ProviderHelper: IProviderHelper
    {
        public ProviderDTO ConvertProviderModelToProviderDTO(ProviderModel model)
        {

            ProviderDTO providerDto = new ProviderDTO()
            {
                Id = model.Id,
                Email = model.Email,
                Info = model.Info,
                IsActive = model.IsActive,
                IsFavorite = model.IsFavorite,
                Name = model.Name,
                Path = model.Path.Replace(PathAPIConstants.API_URL + PathAPIConstants.API_PATH_FILES, string.Empty),
                TimeWorkTo = Convert.ToDateTime(model.TimeWorkTo),
                TimeWorkWith = Convert.ToDateTime(model.TimeWorkWith),
                WorkingDays = model.WorkingDays
            };

            return providerDto;
        }
    }
}
