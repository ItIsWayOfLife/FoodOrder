using API.Interfaces;
using API.Models.Catalog;
using AutoMapper;
using Core.DTO;

namespace API.Helpers
{
    public class CatalogHelper: ICatalogHelper
    {
        public CatalogDTO ConvertCatalogModelToCatalogDTO(CatalogModel model)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<CatalogModel, CatalogDTO>()).CreateMapper();
            return mapper.Map<CatalogModel, CatalogDTO>(model);
        }
    }
}
