using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<MovieViewModel>> GetFavoriteMovies(Guid userId)
        {
            var favoriteMovieIds = await _context.FavoriteMovieList
                .Where(fml => fml.UserId == userId)
                .Select(fml => fml.MovieId)
                .ToListAsync();

            var movies = await _context.Movies
                .Where(m => favoriteMovieIds.Contains(m.Id))
                .Select(m => new MovieViewModel
                {
                    Id = m.Id,
                    Title = m.Title,
                    Year = m.Year,
                    Rating = m.Rating == null ? null : new RatingViewModel
                    {
                        MovieId = m.Id,
                        DbValue = m.Rating.RatingValue,
                        Votes = m.Rating.Votes
                    },
                    Stars = m.Stars.Select(s => new PersonViewModel
                    {
                        Id = s.Person.Id,
                        Name = s.Person.Name,
                        BirthYear = s.Person.Birth
                    }).ToList(),
                    Directors = m.Directors.Select(d => new PersonViewModel
                    {
                        Id = d.Person.Id,
                        Name = d.Person.Name,
                        BirthYear = d.Person.Birth
                    }).ToList()
                })
                .ToListAsync();
            
            return movies;
        }
        
        public async Task<bool> RemoveMovieFromFavorites(Guid userId, int movieId)
        {
            try
            {
                var favorite = await _context.FavoriteMovieList
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.MovieId == movieId);
    
                if (favorite != null)
                {
                    _context.FavoriteMovieList.Remove(favorite);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            
            return false;
        }
        
        public async Task AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }
    }
}