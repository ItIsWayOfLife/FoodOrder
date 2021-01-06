using System.Collections.Generic;
using Web.Models.Provider;

namespace Web.Interfaces
{
    public interface IProviderHelper
    {
        IEnumerable<ProviderViewModel> GetProviders();
        IEnumerable<ProviderViewModel> GetProvidersFavorite();
        List<string> GetSearchSelection(bool isAdmin);
    }
}
