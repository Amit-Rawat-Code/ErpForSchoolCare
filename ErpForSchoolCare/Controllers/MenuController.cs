using ErpForSchoolCare.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json;

namespace ErpForSchoolCare.Controllers
{
    public class MenuController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly UserContext userContext;

        // GET: MenuController
        public MenuController(IHttpClientFactory httpClientFactory,UserContext userContext)
        {
            this.httpClientFactory = httpClientFactory;
            this.userContext = userContext;
        }
        public async Task<ActionResult> Index()
        {
            //List<MenuDto> menus = new List<MenuDto>();
            var client = httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("menus");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<MenuDto>());
                //var data = await response.Content
                //.ReadFromJsonAsync<Dictionary<string, JsonElement>>();
                //if (data != null && data.TryGetValue("ResultSet1", out var menuJson))
                //{
                //    menus = JsonSerializer.Deserialize<List<MenuDto>>(menuJson.GetRawText()) ?? new List<MenuDto>();
                //}
            }
            var menus = await response.Content.ReadFromJsonAsync<List<MenuDto>>();
            return View(menus);
        }

        // GET: MenuController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MenuController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MenuController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MenuDto menu)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    menu.CreatedBY = userContext.UserId.ToString();
                    var client = httpClientFactory.CreateClient("ApiClient");
                    var response = await client.PostAsJsonAsync("menus", menu);
                    if (response.IsSuccessStatusCode)
                    {

                    }   
                }
                return RedirectToAction("Index", "Menu");
            }
            catch
            {
                return View();
            }
        }

        // GET: MenuController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var client = httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("menus/" + id);
            if (!response.IsSuccessStatusCode)
            {
                return View(new MenuDto());                
            }

            //MenuDto menus = new MenuDto();
            //var data = await response.Content
            //.ReadFromJsonAsync<Dictionary<string, JsonElement>>();
            //if (data != null && data.TryGetValue("ResultSet1", out var menuJson))
            //{
            //    menus = JsonSerializer.Deserialize<MenuDto>(menuJson.GetRawText()) ?? new MenuDto();
            //}
            var menus = await response.Content.ReadFromJsonAsync<MenuDto>();
            return View(menus);
        }

        // POST: MenuController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MenuController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MenuController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
