namespace ErpForSchoolCare.ViewComponent
{
    using ErpForSchoolCare.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Text.Json;

    public class SidebarMenuViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SidebarMenuViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            var response = await client.GetAsync("menus");

            if (!response.IsSuccessStatusCode)
            {
                return View("Menus", new List<MenuDto>());
            }

            var menus = await response.Content.ReadFromJsonAsync<List<MenuDto>>();
            return View("Menus", menus ?? new List<MenuDto>());
        }

    }
}
