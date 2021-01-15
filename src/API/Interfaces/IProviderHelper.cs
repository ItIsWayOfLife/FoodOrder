using API.Models.Provider;
using Core.DTO;

namespace API.Interfaces
{
    public interface IProviderHelper
    {
        ProviderDTO ConvertProviderModelToProviderDTO(ProviderModel model);
    }
}
