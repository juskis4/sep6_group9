using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using webApplication.Models;

namespace webApplication.Services
{
    public interface IAuthenticationService
    {
        Task SignInUserAsync(User user, HttpContext httpContext);
    }

}