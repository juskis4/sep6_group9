using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using webApplication.Data;
using webApplication.Interfaces;
using webApplication.ViewModels;

namespace webApplication.Services
{
    public class MovieService : IMovieService
    {
        private readonly IHttpClientWrapper _httpClient;
        private readonly IMovieDataContext  _context;
        private const int PageSize = 12;
        private readonly string _apiKey = "220d1d34";

        public MovieService(IHttpClientWrapper  httpClient, IMovieDataContext  context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public async Task<MovieDetailsViewModel> GetMovieDetailsAsync(int movieId)
        {
            string apiUrl = $"http://www.omdbapi.com/?i=tt{movieId}&plot=full&apikey={_apiKey}";
            var response = await _httpClient.GetAsync(apiUrl);
    
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<MovieDetailsViewModel>(content);
            }

            throw new Exception("Error retrieving movie details.");
        }

        public async Task<string?> GetMoviePosterAsync(int movieId)
        {
            string apiUrl = $"http://www.omdbapi.com/?i=tt{movieId}&plot=full&apikey={_apiKey}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<MovieDetailsViewModel>(content)?.Poster;
            }

            throw new Exception("Error retrieving movie details.");
        }

        public async Task<MovieListViewModel> GetSearchResultsAsync(string query, int page)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Query cannot be empty.", nameof(query));
            }
            
            var totalMoviesCount = await _context.Movies
                .Where(m => EF.Functions.ILike(m.Title, $"%{query}%"))
                .CountAsync();

            var totalPages = (int)Math.Ceiling(totalMoviesCount / (double)PageSize);
            
            page = Math.Max(1, Math.Min(page, totalPages));
            
            var movies = await _context.Movies
                .Where(m => EF.Functions.ILike(m.Title, $"%{query}%"))
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

            var model = new MovieListViewModel
            {
                Movies = movies,
                CurrentPage = page,
                TotalPages = totalPages,
                Search = query
            };

            return model;
        }
    }

}
