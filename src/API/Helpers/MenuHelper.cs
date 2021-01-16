using API.Interfaces;
using API.Models.Menu;
using Core.DTO;
using System;
using System.Collections.Generic;

namespace API.Helpers
{
    public class MenuHelper : IMenuHelper
    {
        public MenuDTO ConvertMenuModelToMenuDTO(MenuModel model)
        {
            return new MenuDTO()
            {
                Id = model.Id,
                Info = model.Info,
                Date = Convert.ToDateTime(model.Date),
                ProviderId = model.ProviderId
            };
        }

        public MenuModel ConvertMenuDTOToMenuModel(MenuDTO menuDTO)
        {
            return new MenuModel()
            {
                Id = menuDTO.Id,
                Info = menuDTO.Info,
                ProviderId = menuDTO.ProviderId,
                Date = menuDTO.Date.ToShortDateString()
            };
        }

        public IEnumerable<MenuModel> ConvertMenuDTOsToMenuModels(IEnumerable<MenuDTO> menuDTOs)
        {
            var menuModels = new List<MenuModel>();

            foreach (var m in menuDTOs)
            {
                menuModels.Add(ConvertMenuDTOToMenuModel(m));
            }

            return menuModels;
        }
    }
}
