using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using webApplication.Data;
using webApplication.Models;
using webApplication.ViewModels;

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

        public async Task<bool> VerifyUser(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            return user != null;
        }

        public async Task<bool> RegisterUser(RegisterViewModel model)
        {
            var user = new User
            {
                UserId = Guid.NewGuid(), 
                Username = model.Username,
                Password = model.Password 
            };

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> AddMovieToFavoriteList(Guid userId, int movieId)
        {
            if (!await IsMovieAddedToList(userId, movieId))
            {
                try
                {
                    var userMovieList = new UserMovieList
                    {
                        UserId = userId,
                        MovieId = movieId,
                        Type = 'F' // 'F' for watchlist
                    };

                    _context.FavoriteMovieList.Add(userMovieList);
                    await _context.SaveChangesAsync();

                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
            return false;
        }

        public async Task<bool> IsMovieAddedToList(Guid userId, int movieId)
        {
            var instance = await _context.FavoriteMovieList.FirstOrDefaultAsync
                                        (uml => uml.UserId == userId && uml.MovieId == movieId);
            return instance != null;
        }
    }
}