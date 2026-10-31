using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ErpForSchoolCare.Filter
{
    public class UnauthorizedRedirectFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var result = await next();
            if (context.HttpContext.Response.StatusCode == 401)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }
    }
}
