using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using webApplication.Data;
using webApplication.Models;

namespace webApplication.Services
{
    public class UserService : IUserService
    {
        private readonly MovieDataContext _context;

        public UserService(MovieDataContext context)
        {
            _context = context;
        }

        public async Task<User> ValidateUserAsync(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user != null && VerifyPassword(password, user.Password))
            {
                return user;
            }

            return null;
        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            return enteredPassword == storedPassword; 
        }
    }
}