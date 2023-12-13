using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using webApplication.Data;
using webApplication.Models;
using webApplication.ViewModels;

namespace webApplication.Services
{
    /// <summary>
    /// Service providing user-related operations such as validation, registration, commenting,
    /// and managing favorite movies
    /// </summary>
    public class UserService : IUserService
    {
        private readonly MovieDataContext _context;

        public UserService(MovieDataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Validates a user's credentials
        /// </summary>
        /// <param name="username">User's username</param>
        /// <param name="password">User's password</param>
        /// <returns>The validated user or null if validation fails</returns>
        public async Task<User?> ValidateUserAsync(string username, string password)
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
            //Placeholder for future password hashing function
            return enteredPassword == storedPassword; 
        }

        /// <summary>
        /// Verifies if a user exists based on username
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>Boolean indicating whether the user exists</returns>
        public async Task<bool> VerifyUser(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            return user != null;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="model">Registration details</param>
        /// <returns>Boolean indicating successful registration</returns>
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

        /// <summary>
        /// Adds a movie to a user's favorite list
        /// </summary>
        /// <param name="userId">User's ID</param>
        /// <param name="movieId">Movie's ID</param>
        /// <returns>Boolean indicating if the movie was successfully added</returns>
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

        /// <summary>
        /// Checks if movie is already in the list
        /// </summary>
        /// <param name="userId">User's ID</param>
        /// <param name="movieId">Movie's ID</param>
        /// <returns>Boolean indicating if the movie is already in the favorite list</returns>
        private async Task<bool> IsMovieAddedToList(Guid userId, int movieId)
        {
            var instance = await _context.FavoriteMovieList.FirstOrDefaultAsync
                                        (uml => uml.UserId == userId && uml.MovieId == movieId);
            return instance != null;
        }

        /// <summary>
        /// Retrieves a user's favorite movies
        /// </summary>
        /// <param name="userId">User's ID</param>
        /// <returns>List of favorite movies</returns>
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
        
        /// <summary>
        /// Removes a movie from a user's favorites
        /// </summary>
        /// <param name="userId">User's ID</param>
        /// <param name="movieId">Movie's ID</param>
        /// <returns>Boolean indicating successful removal</returns>
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
        
        /// <summary>
        /// Adds a comment to a movie
        /// </summary>
        /// <param name="comment">Comment to be added</param>
        /// <returns>Task</returns>
        public async Task AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }
    }
}