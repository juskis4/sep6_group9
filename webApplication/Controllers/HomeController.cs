    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using webApplication.Models;
    using webApplication.Services;
    using webApplication.ViewModels;

    namespace webApplication.Controllers
    {
        /// <summary>
        /// Controller responsible for handling the main functionalities of the application,
        /// including viewing, searching, and managing movies and user preferences
        /// </summary>
        [Authorize]
        public class HomeController : Controller
        {
            private readonly ILogger<HomeController> _logger;
            private readonly IMovieService _movieService;
            private readonly IMovieDbService _movieDbService;
            private readonly IUserService _userService;

            private const int PageSize = 12;

            /// <summary>
            /// Initializes a new instance of the HomeController class
            /// </summary>
            /// <param name="logger">Logger for logging information and errors</param>
            /// <param name="movieService">Service for movie-related operations</param>
            /// <param name="movieDbService">Service for database-related operations on movies</param>
            /// <param name="userService">Service for user-related operation</param>
            public HomeController(ILogger<HomeController> logger, IMovieService movieService,
                IMovieDbService movieDbService, IUserService userService)
            {
                _logger = logger;
                _movieService = movieService;
                _movieDbService = movieDbService;
                _userService = userService;
            }

            /// <summary>
            /// Displays the main page with a list of movies and pagination
            /// </summary>
            /// <param name="page">Current page number for pagination</param>
            /// <param name="year">Optional filter by year</param>
            /// <param name="minRating">Optional filter by minimum rating</param>
            /// <returns>Home view</returns>
            [HttpGet]
            public async Task<IActionResult> Index(int page = 1, int? year = null, double? minRating = null)
            {
                var totalMoviesCount = await _movieDbService.GetMovieCountAsync(year, minRating);
                var totalPages = (int) Math.Ceiling(totalMoviesCount / (double) PageSize);

                page = Math.Max(1, Math.Min(page, totalPages));

                var movies = await _movieDbService.GetMoviesWithPagination(page, PageSize, year, minRating);

                if (year.HasValue)
                {
                    movies = movies.Where(m => m.Year == year.Value);
                }
                
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
                
                //For year select dropdown
                ViewBag.Years = await _movieDbService.GetMovieYears();
                if (year != null) ViewBag.SelectedYear = year;
                
                //For rating filtering
                if (minRating != null) ViewBag.MinRating = minRating;
                
                return View(model);
            }
            
            /// <summary>
            /// Retrieves and sets the poster for a given movie
            /// </summary>
            /// <param name="movie">The movie for which to set the poster</param>
            /// <returns>Task</returns>
            [HttpGet]
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

            /// <summary>
            /// Displays the details of a specific movie
            /// </summary>
            /// <param name="id">The ID of the movie to display details for</param>
            /// <returns>Details view</returns>
            [HttpGet]
            public async Task<IActionResult> Details(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var movieViewModel = await _movieDbService.GetMovieAsync(id.Value);
        
                try
                {
                    var movieDetails = await _movieService.GetMovieDetailsAsync(id.Value);
                    movieViewModel.Details = movieDetails;

                    var comments = await _movieDbService.GetCommentsByMovieIdAsync(id.Value);

                    movieViewModel.Comments = comments.Select(c => new CommentViewModel
                    {
                        CommentId = c.CommentId,
                        Username = c.Username, 
                        Content = c.Content,
                    }).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving movie details.");
                    return NotFound();
                }
        
                return View(movieViewModel);
            }

            /// <summary>
            /// Displays an error page in case of a request failure
            /// </summary>
            /// <returns>The Error view</returns>
            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            public IActionResult Error()
            {
                return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
            }

            /// <summary>
            /// Performs a search for movies based on a query and displays the results
            /// </summary>
            /// <param name="query">The search query string</param>
            /// <param name="page">Current page number for pagination in search results</param>
            /// <returns>The Index view with search results</returns>
            [HttpGet]
            public async Task<IActionResult> Search(string query, int page = 1)
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return RedirectToAction(nameof(Index));
                }

                var totalMoviesCount = await _movieDbService.GetSearchedMoviesCountAsync(query);
                var totalPages = (int) Math.Ceiling(totalMoviesCount / (double) PageSize);
                page = Math.Max(1, Math.Min(page, totalPages));
                
                var movies = await _movieDbService.GetSearchedMoviesWithPaginationAsync(query, page, PageSize);

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

                var fetchPosterTasks = model.Movies.Select(SetMoviePoster).ToList();
                
                await Task.WhenAll(fetchPosterTasks);
                
                return View("Index", model);
            }
            
            /// <summary>
            /// Adds a movie to the user's list of favorites
            /// </summary>
            /// <param name="movieId">The ID of the movie to add to favorites</param>
            /// <returns>Index view</returns>
            [HttpPost]
            public async Task<IActionResult> AddToFavoritelist(int movieId)
            {
                if (!User.Identity.IsAuthenticated)
                {
                    // Trow error message to login again
                    return RedirectToAction("Login", "User");
                }
                
                var userIdClaim = User.FindFirst(ClaimTypes.Name);
                
                if (userIdClaim == null)
                {
                    // Trow error message to login again
                    return RedirectToAction("Login", "User");
                }
                
                var userId = Guid.Parse(userIdClaim.Value);

                var isAdded = await _userService.AddMovieToFavoriteList(userId, movieId);

                if (isAdded)
                {
                    TempData["Message"] = "Movie added to watchlist.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Movie is already in the Favorites list.";
                }

                return RedirectToAction("Index");
            }


        }
    }