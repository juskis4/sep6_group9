using System.Threading.Tasks;
using webApplication.Models;

namespace webApplication.Services
{
    public interface IUserService
    {
        Task<User> ValidateUserAsync(string username, string password);
    }
}