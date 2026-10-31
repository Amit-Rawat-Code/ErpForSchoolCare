using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolApis.Model;
using SchoolApis.Services;
using System.Security.Claims;

namespace SchoolApis.Controllers
{
    [Route("api/menus")]
    [ApiController]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService menuService;

        public MenuController(IMenuService menuService)
        {
            this.menuService = menuService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMenus()
        {
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            int Action = 1;
            if (string.IsNullOrEmpty(role))
                return Unauthorized();
            var menus = await menuService.GetMenusByRoleAsync(role, Action);
            return Ok(menus);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMenuById(int id)
        {
            int Action = 2;
            var menus = await menuService.GetMenusByIdAsync(id,Action);
            return Ok(menus);
        }
        [HttpPost]
        public async Task<IActionResult> CreateMenu(MenuDto menu)
        {
            menu.Action = 1;
            int result = await menuService.PostMenu(menu);
            return result switch
            {
                1 => Created("", "Menu created successfully"),
                4 => Conflict("Menu already exists"),
                _ => BadRequest("Operation failed")
            };
        }
        [HttpPost("update-menu")]
        public async Task<IActionResult> UpdateMenu(MenuDto menu)
        {
            menu.Action = 2;
            int result = await menuService.PostMenu(menu);
            return result switch
            {
                1 => Created("", "Menu created successfully"),
                -1 => NotFound("Menu not found"),
                _ => BadRequest("Operation failed")
            };
        }
        [HttpPost("delete-menu")]
        public async Task<IActionResult> DeleteMenu(MenuDto menu)
        {
            menu.Action = 3;
            int result = await menuService.PostMenu(menu);
            return result switch
            {
                1 => Created("", "Menu created successfully"),
                -1 => NotFound("Menu not found"),
                _ => BadRequest("Operation failed")
            };
        }
    }
}
