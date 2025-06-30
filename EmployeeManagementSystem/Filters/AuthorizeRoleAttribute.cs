using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeeManagementSystem.Filters
{
    public class AuthorizeEmployeeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Just check if UserId is present in session
            var userId = context.HttpContext.Session.GetInt32("UserId");

            if (userId == null || userId <= 0)
            {
                // Not logged in, redirect to login page
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }
    }
}
