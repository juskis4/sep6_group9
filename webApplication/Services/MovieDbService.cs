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
    public class MovieDbService : IMovieDbService
    {
        private readonly MovieDataContext _context;
    
        public MovieDbService(MovieDataContext context)
        {
            _context = context;
        }
    
        // public async Task<int> GetTotalMoviesCountAsync()
        // {
        //     return await _context.Movies.CountAsync();
        // }
        //
        // public async Task<List<MovieViewModel>> GetMoviesAsync(int skip, int take)
        // {
        //     return await _context.Movies
        //         .OrderBy(m => m.Title)
        //         .Skip(skip)
        //         .Take(take)
        //         .Select(m => new MovieViewModel
        //         {
        //             // Your existing conversion logic here...
        //         })
        //         .ToListAsync();
        // }
        //
        // public async Task<MovieViewModel> GetMovieAsync(int? id)
        // {
        //     var movie = await _context.Movies
        //         .Include(m => m.Rating)
        //         //TODO: Add other necessary includes for related data
        //         .FirstOrDefaultAsync(m => m.Id == id);
        //
        //     if (movie == null) return null;
        //
        //     // Conversion logic here...
        // }
        //
        // public async Task<IEnumerable<MovieViewModel>> SearchMoviesAsync(string query, int skip, int take)
        // {
        //     IQueryable<Movie> movieQuery = _context.Movies.Where(m => EF.Functions.ILike(m.Title, $"%{query}%"));
        //
        //     return await movieQuery
        //         .OrderBy(m => m.Title)
        //         .Skip(skip)
        //         .Take(take)
        //         .Select(m => new MovieViewModel
        //         {
        //             // Your existing conversion logic here...
        //         })
        //         .ToListAsync();
        // }
        //
        public async Task<IQueryable<Movie>> GetSearchedMoviesAsync(string query)
        {
            IQueryable<Movie> movieQuery = _context.Movies.Where(m => EF.Functions.ILike(m.Title, $"%{query}%"));
            return movieQuery;
        }
        
        public async Task<int> GetMovieCountAsync()
        {
            return await _context.Movies.CountAsync();
        }

        public async Task<List<MovieViewModel>> GetMoviesWithPagination(int page, int PageSize = 12)
        {
            var movies = await _context.Movies
                .OrderBy(m => m.Title)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(m =>
                    new MovieViewModel
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

        public async Task<MovieViewModel> GetMovieAsync(int? id)
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
    }
}
