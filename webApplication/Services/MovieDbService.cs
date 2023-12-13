using Microsoft.EntityFrameworkCore;
using webApplication.Data;
using webApplication.Models;
using webApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webApplication.Services
{
    /// <summary>
    /// Service providing database operations for movies and its related movie data
    /// </summary>
    public class MovieDbService : IMovieDbService
    {
        private readonly MovieDataContext _context;
    
        public MovieDbService(MovieDataContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Retrieves a paginated list of movies based on a search query
        /// </summary>
        /// <param name="query">Search query string</param>
        /// <param name="page">Page number for pagination</param>
        /// <param name="PageSize">Number of items per page</param>
        /// <returns>A list of movie view models</returns>
        public async Task<IEnumerable<MovieViewModel>> GetSearchedMoviesWithPaginationAsync(string query, int page, int PageSize = 12)
        {
            var movieQuery = _context.Movies.Where(m => EF.Functions.ILike(m.Title, $"%{query}%")).AsQueryable();
            
            var movies = await movieQuery
                .OrderBy(m => m.Title)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(m => new MovieViewModel
                {
                    Id = m.Id,
                    Title = m.Title,
                    Year = m.Year,
                    Rating = m.Rating == null
                        ? null
                        : new RatingViewModel
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
        /// Counts the total number of movies matching a search query
        /// </summary>
        /// <param name="query">Search query string</param>
        /// <returns>Count of movies</returns>
        public async Task<int> GetSearchedMoviesCountAsync(string query)
        {
            var movieCount = await _context.Movies.Where(m => EF.Functions.ILike(m.Title, $"%{query}%")).CountAsync();
            return movieCount;
        }
        
        /// <summary>
        /// Counts the total number of movies optionally filtered by year and/or minimum rating
        /// </summary>
        /// <param name="year">Optional year filter</param>
        /// <param name="minRating">Optional minimum rating filter</param>
        /// <returns>Count of movies</returns>
        public async Task<int> GetMovieCountAsync(int? year = null, double? minRating = null)
        {
            var count = _context.Movies.AsQueryable();
            
            if (year.HasValue)
            {
                count = count.Where(m => m.Year == year.Value);
            }
            if (minRating.HasValue)
            {
                count = count.Where(m => m.Rating.RatingValue >= minRating);
            }

            return await count.CountAsync();

        }

        /// <summary>
        /// Counts the total number of stars
        /// </summary>
        /// <returns>Count of stars</returns>
        public async Task<int> GetStarsCountAsync()
        {
            var count = await _context.Stars.CountAsync();
            return count;
        }
        
        
        /// <summary>
        /// Counts the total number of directors
        /// </summary>
        /// <returns>Count of directors</returns>
        public async Task<int> GetDirectorsCountAsync()
        {
            var count = await _context.Directors.CountAsync();
            return count;
        }

        /// <summary>
        /// Retrieves a paginated list of movies optionally filtered by year and/or minimum rating
        /// </summary>
        /// <param name="page">Page number for pagination</param>
        /// <param name="PageSize">Number of items per page</param>
        /// <param name="year">Optional year filter</param>
        /// <param name="minRating">Optional minimum rating filter</param>
        /// <returns>A list of movie view models</returns>
        public async Task<IEnumerable<MovieViewModel>> GetMoviesWithPagination(int page, int PageSize = 12, 
                                                                            int? year = null, double? minRating = null)
        {
            var query = _context.Movies.AsQueryable();

            if (year.HasValue)
            {
                query = query.Where(m => m.Year == year.Value);
            }

            if (minRating.HasValue)
            {
                query = query.Where(m => m.Rating.RatingValue >= minRating.Value);
            }

        var movies = await query
                .OrderBy(m => m.Title)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(m => new MovieViewModel
                {
                    Id = m.Id,
                    Title = m.Title,
                    Year = m.Year,
                    Rating = m.Rating == null
                        ? null
                        : new RatingViewModel
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
        /// Retrieves a specific movie by its ID
        /// </summary>
        /// <param name="id">ID of the movie</param>
        /// <returns>A movie view model if found, otherwise null</returns>
        public async Task<MovieViewModel?> GetMovieAsync(int? id)
        {
            if (id == null) return null;
            var movie = await _context.Movies
                .Include(m => m.Rating)
                .FirstOrDefaultAsync(m => m.Id == id);
            

            var movieViewModel = new MovieViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Year = movie.Year,
                Rating = movie.Rating != null ? new RatingViewModel
                {
                    MovieId = movie.Id,
                    DbValue = movie.Rating.RatingValue,
                    Votes = movie.Rating.Votes
                } : null, 
                Stars = movie.Stars?.Select(s => new PersonViewModel
                {
                    Id = s.Person.Id,
                    Name = s.Person.Name,
                    BirthYear = s.Person.Birth
                }).ToList() ?? new List<PersonViewModel>(), 
                Directors = movie.Directors?.Select(d => new PersonViewModel
                {
                    Id = d.Person.Id,
                    Name = d.Person.Name,
                    BirthYear = d.Person.Birth
                }).ToList() ?? new List<PersonViewModel>() 
            };
            return movieViewModel;

        }
        
        /// <summary>
        /// Retrieves a distinct list of years in which movies were released
        /// </summary>
        /// <returns>List of years</returns>
        public async Task<List<int?>> GetMovieYears()
        {
            var years = await _context.Movies.Select(m => m.Year).Distinct().OrderBy(y => y).ToListAsync();
            return years;
        }
        
        /// <summary>
        /// Retrieves comments associated with a specific movie
        /// </summary>
        /// <param name="movieId">ID of the movie</param>
        /// <returns>List of comments</returns>
        public async Task<IEnumerable<Comment>> GetCommentsByMovieIdAsync(int movieId)
        {
            return await _context.Comments
                .Where(c => c.MovieId == movieId)
                .ToListAsync();
        }
        
        /// <summary>
        /// Retrieves a paginated list of stars
        /// </summary>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>A person list view model</returns>
        public async Task<PersonListViewModel> GetStarsWithPaginationAsync(int page, int pageSize = 12)
        {
            var paginatedStarIds = await _context.Stars
                .Select(s => s.Person.Id)
                .Distinct()
                .OrderBy(id => id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var paginatedStars = new List<PersonViewModel>();
            foreach (var starId in paginatedStarIds)
            {
                var star = await _context.People.FindAsync(starId);
                var movies = await _context.Stars
                    .Where(s => s.PersonId == starId)
                    .Select(s => s.Movie)
                    .Select(m => new MovieViewModel
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Year = m.Year,
                    })
                    .ToListAsync();

                paginatedStars.Add(new PersonViewModel
                {
                    Id = star.Id,
                    Name = star.Name,
                    BirthYear = star.Birth,
                    Movies = movies
                });
            }
            
            var personListViewModel = new PersonListViewModel
            {
                People = paginatedStars
            };

            return personListViewModel;
        }
        
        /// <summary>
        /// Retrieves a paginated list of directors
        /// </summary>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>A person list view model</returns>
        public async Task<PersonListViewModel> GetDirectorsWithPaginationAsync(int page, int pageSize = 12)
        {
            var paginatedDirectorIds = await _context.Directors
                .Select(d => d.Person.Id)
                .Distinct()
                .OrderBy(id => id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var paginatedDirectors = new List<PersonViewModel>();
            foreach (var directorId in paginatedDirectorIds)
            {
                var director = await _context.People.FindAsync(directorId);
                var movies = await _context.Directors
                    .Where(d => d.PersonId == directorId)
                    .Select(d => d.Movie)
                    .Select(m => new MovieViewModel
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Year = m.Year,
                    })
                    .ToListAsync();

                paginatedDirectors.Add(new PersonViewModel
                {
                    Id = director.Id,
                    Name = director.Name,
                    BirthYear = director.Birth, 
                    Movies = movies
                });
            }

            var personListViewModel = new PersonListViewModel
            {
                People = paginatedDirectors,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)_context.Directors.Count() / pageSize)
            };

            return personListViewModel;
        }


    }
}
