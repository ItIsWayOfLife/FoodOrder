using API.Models.Catalog;
using Core.DTO;

namespace API.Interfaces
{
    public interface ICatalogHelper
    {
        CatalogDTO ConvertCatalogModelToCatalogDTO(CatalogModel model);
    }
}
