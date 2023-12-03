using System;
using System.Threading.Tasks;
using webApplication.Models;
using webApplication.ViewModels;

namespace webApplication.Services
{
    public interface IUserService
    {
        Task<User> ValidateUserAsync(string username, string password);

        Task<bool> VerifyUser(string username);

        Task<bool> RegisterUser(RegisterViewModel model);

        Task<bool> AddMovieToFavoriteList(Guid userId, int movieId);
    }
}