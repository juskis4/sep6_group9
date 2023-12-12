using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using webApplication.Models;

namespace webApplication.Services
{
    /// <summary>
    /// Provides authentication services for signing in users
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// Signs in a user asynchronously
        /// </summary>
        /// <param name="user">The user to be signed in</param>
        /// <param name="httpContext">The current HTTP context</param>
        /// <returns>task</returns>
        public async Task SignInUserAsync(User user, HttpContext httpContext)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserId.ToString()),
                new Claim("Username", user.Username)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true
            };

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }
    }

}