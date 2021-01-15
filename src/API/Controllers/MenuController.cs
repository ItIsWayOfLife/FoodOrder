using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{


    public class Menu
    {   /// <summary>
    /// Id menu
    /// </summary>
        public string Id {get;set;}

        /// <summary>
        /// Name menu
        /// </summary>
        public string Name { get; set; }
        }
    /// <summary>
    /// Menu controller 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
            List<Menu> menus = new List<Menu>() {
         new Menu() { Id ="1" , Name = "Menu1"},
           new Menu() { Id ="2" , Name = "Menu2"},
             new Menu() { Id ="3" , Name = "Menu3"}
        };

        /// <summary>
        /// Get menus
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            
            return new ObjectResult(menus);
        }

        /// <summary>
        /// Add menu
        /// </summary>
        /// <param name="item"></param>
        /// <returns>List menus</returns>
        [HttpPost]
        public IActionResult Post(Menu item)
        {
            menus.Add(item);

            return Ok(item);
        }
    }
}
