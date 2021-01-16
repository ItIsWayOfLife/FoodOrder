using API.Models.Menu;
using Core.DTO;
using System.Collections.Generic;

namespace API.Interfaces
{
    public interface IMenuHelper
    {
        MenuDTO ConvertMenuModelToMenuDTO(MenuModel model);
        MenuModel ConvertMenuDTOToMenuModel(MenuDTO dto);

        IEnumerable<MenuModel> ConvertMenuDTOsToMenuModels(IEnumerable<MenuDTO> dtos);
    }
}
