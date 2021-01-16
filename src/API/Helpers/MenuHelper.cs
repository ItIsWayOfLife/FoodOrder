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

        public MenuModel ConvertMenuDTOToMenuModel(MenuDTO dto)
        {
            return new MenuModel()
            {
                Id = dto.Id,
                Info = dto.Info,
                ProviderId = dto.ProviderId,
                Date = dto.Date.ToShortDateString()
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
