using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CompanyWebApplication.Security
{
    public class BasicAuthorizeFilter : IAuthorizationFilter
    {
        private readonly string _realm;

        public BasicAuthorizeFilter(string realm = null)
        {
            _realm = realm;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string authHeader = context.HttpContext.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                var encodedUsernamePassword =
                    authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();
                var decodedUsernamePassword =
                    Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var username = decodedUsernamePassword.Split(':', 2)[0];
                var password = decodedUsernamePassword.Split(':', 2)[1];
                if (IsAuthorized(username, password))
                {
                    return;
                }
            }

            context.HttpContext.Response.Headers["WWW-Authenticate"] = "Basic";
            if (!string.IsNullOrWhiteSpace(_realm))
            {
                context.HttpContext.Response.Headers["WWW-Authenticate"] += $" realm=\"{_realm}\"";
            }

            context.Result = new UnauthorizedResult();
        }

        public bool IsAuthorized(string username, string password)
        {
            return username.Equals("user", StringComparison.InvariantCultureIgnoreCase)
                   && password.Equals("pass");
        }
    }
}