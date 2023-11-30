using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using webApplication.Models;
using webApplication.Services;
using webApplication.ViewModels;

namespace webApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService _movieService;
        private readonly IMovieDbService _movieDbService;

        private const int PageSize = 12;

        public HomeController(ILogger<HomeController> logger, IMovieService movieService,
                                IMovieDbService movieDbService)
        {
            _logger = logger;
            _movieService = movieService;
            _movieDbService = movieDbService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var totalMoviesCount = await _movieDbService.GetMovieCountAsync();
            var totalPages = (int) Math.Ceiling(totalMoviesCount / (double) PageSize);

            page = Math.Max(1, Math.Min(page, totalPages));

            var movies = await _movieDbService.GetMoviesWithPagination(page, PageSize);
            
            var model = new MovieListViewModel
            {
                Movies = movies,
                CurrentPage = page,
                TotalPages = totalPages
            };
            
            foreach (var movie in model.Movies)
            {
                movie.Details = new MovieDetailsViewModel(); 
            }
            
            var fetchPosterTasks = model.Movies.Select(movie => SetMoviePoster(movie)).ToList();
            
            await Task.WhenAll(fetchPosterTasks);


            return View(model);
        }
        
        private async Task SetMoviePoster(MovieViewModel movie)
        {
            string defaultPosterUrl = "https://motivatevalmorgan.com/wp-content/uploads/2016/06/default-movie.jpg"; 

            try
            {
                var poster = await _movieService.GetMoviePosterAsync(movie.Id);
                if (string.IsNullOrEmpty(poster))
                {
                    movie.Details.Poster = defaultPosterUrl;
                }

                movie.Details.Poster = poster;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving poster for movie ID {MovieId}. Using default poster.", movie.Id);
                movie.Details.Poster = defaultPosterUrl;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MovieViewModel movieViewModel = await _movieDbService.GetMovieAsync(id);
            
            try
            {
                var movieDetails = await _movieService.GetMovieDetailsAsync(id.Value);
                movieViewModel.Details = movieDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving movie details.");
                return NotFound();
            }
            
            return View(movieViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public async Task<IActionResult> Search(string query, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction(nameof(Index));
            }

            IQueryable<Movie> movieQuery = await _movieDbService.GetSearchedMoviesAsync(query);

            var totalMoviesCount = await movieQuery.CountAsync();
            var totalPages = (int) Math.Ceiling(totalMoviesCount / (double) PageSize);
            page = Math.Max(1, Math.Min(page, totalPages));

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

            var model = new MovieListViewModel
            {
                Movies = movies,
                CurrentPage = page,
                TotalPages = totalPages,
                Search = query
            };
            
            foreach (var movie in model.Movies)
            {
                movie.Details = new MovieDetailsViewModel(); 
            }
            
            var fetchPosterTasks = model.Movies.Select(movie => SetMoviePoster(movie)).ToList();
            
            await Task.WhenAll(fetchPosterTasks);
            
            return View("Index", model);
        }
    }
}