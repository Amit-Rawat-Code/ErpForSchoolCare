using ErpForSchoolCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ErpForSchoolCare.Controllers
{
    [Authorize] // ⭐ REQUIRED
    public class DashBoardController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        //private readonly UserContext userContext;

        public DashBoardController(
            IHttpClientFactory httpClientFactory
            /*,UserContext userContext*/)
        {
            this.httpClientFactory = httpClientFactory;
            //this.userContext = userContext;
        }

        public IActionResult Dashboard()
        {
            // Claims are now available
            //var userName = userContext.UserName;
            //var userId = userContext.UserId;
            //var roles = userContext.Roles;
            //var tokenExpiry = userContext.TokenExpiry;

            return View();
        }
    }
}
