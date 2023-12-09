using Microsoft.EntityFrameworkCore;
using webApplication.Data;
using webApplication.Models;
using webApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace webApplication.Services
{
    public class MovieDbService : IMovieDbService
    {
        private readonly MovieDataContext _context;
    
        public MovieDbService(MovieDataContext context)
        {
            _context = context;
        }
        
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

        public async Task<int> GetSearchedMoviesCountAsync(string query)
        {
            var movieCount = await _context.Movies.Where(m => EF.Functions.ILike(m.Title, $"%{query}%")).CountAsync();
            return movieCount;
        }
        
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

        public async Task<int> GetStarsCountAsync()
        {
            var count = await _context.Stars.CountAsync();
            return count;
        }
        
        public async Task<int> GetDirectorsCountAsync()
        {
            var count = await _context.Directors.CountAsync();
            return count;
        }

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


        public async Task<MovieViewModel?> GetMovieAsync(int? id)
        {
            if (id != null)
            {
                var movie = await _context.Movies
                    .Include(m => m.Rating) 
                    //TODO Add other necessary includes for related data
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
            
            return null;
        }
        
        public async Task<List<int?>> GetMovieYears()
        {
            var years = await _context.Movies.Select(m => m.Year).Distinct().OrderBy(y => y).ToListAsync();
            return years;
        }
        
        public async Task<IEnumerable<Comment>> GetCommentsByMovieIdAsync(int movieId)
        {
            return await _context.Comments
                .Where(c => c.MovieId == movieId)
                .ToListAsync();
        }
        
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
