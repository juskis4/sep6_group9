using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webApplication.Models;
using webApplication.Services;
using webApplication.ViewModels;
using IAuthenticationService = webApplication.Services.IAuthenticationService;

namespace webApplication.Controllers
{   
    /// <summary>
    /// Controller responsible for handling user-related actions such as authentication, commenting,
    /// registration and favorite list management
    /// </summary>
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMovieService _movieService;
        private readonly IAuthenticationService _authenticationService;
        
        /// <summary>
        /// Initializes a new instance of the UserController class
        /// </summary>
        /// <param name="userService">Service for user-related operations</param>
        /// <param name="movieService">Service for movie-related operations</param>
        /// <param name="authenticationService">Service for authentication-related operations</param>
        public UserController(IUserService userService, IMovieService movieService, IAuthenticationService authenticationService)
        {
            _userService = userService;
            _movieService = movieService;
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Presents the login view to the user
        /// </summary>
        /// <returns>Login page view</returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.HideNavBar = true;
            return View();
        }
        
        /// <summary>
        /// Signs out the current user and redirects to the login page
        /// </summary>
        /// <returns>Login View</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
        }

        /// <summary>
        /// Attempts to log in a user with provided credentials
        /// </summary>
        /// <param name="model">The user's login information</param>
        /// <returns>Home View</returns>
        [HttpPost]
        public async Task<IActionResult> Login(UserViewModel model)
        {
            ViewBag.HideNavBar = true;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userService.ValidateUserAsync(model.Username, model.Password);
            if (user != null)
            {
                await _authenticationService.SignInUserAsync(user, HttpContext);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        /// <summary>
        /// Presents the registration view to the user
        /// </summary>
        /// <returns>A view that renders the registration page</returns>
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.HideNavBar = true;
            return View();
        }

        /// <summary>
        /// Attempts to register a new user with provided details
        /// </summary>
        /// <param name="model">The registration details</param>
        /// <returns>Home View + RegisterViewModel</returns>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewBag.HideNavBar = true;
            if (ModelState.IsValid)
            {
                
                // Check if username already exists
                var userExists = await _userService.VerifyUser(model.Username);
                if (userExists)
                {
                    ModelState.AddModelError("Username", "Username already taken!");
                    return View(model);
                }

                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("Password", "Passwords do not match!");
                    return View(model);
                }

                var registerUser = await _userService.RegisterUser(model);
                if (registerUser)
                {
                    return RedirectToAction("Login", "User");
                }
                
            }
            
            return View(model);
        }

        /// <summary>
        /// Displays the profile page with the user's favorite movie list
        /// </summary>
        /// <returns>Profile view</returns>
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Redirect to login or handle the case when the user is not logged in
                return RedirectToAction("Login", "User");
            }

            // Retrieve the UserId claim
            var userIdClaim = User.FindFirst(ClaimTypes.Name);

            // Check if the UserId claim exists and is not null
            if (userIdClaim == null)
            {
                // Handle the case when the UserId claim is not found
                // This could be a redirect or showing an error message
                return RedirectToAction("Index", "Home");
            }

            // Parse the UserId claim value to Guid
            var userId = Guid.Parse(userIdClaim.Value);
            
            var favoriteMovies = await _userService.GetFavoriteMovies(userId);
            
            var model = new MovieListViewModel
            {
                Movies = favoriteMovies,
            };
            
            foreach (var movie in model.Movies)
            {
                movie.Details = new MovieDetailsViewModel(); 
            }

            foreach (var movie in model.Movies)
            {
                movie.Details.Poster = await _movieService.GetMoviePosterAsync(movie.Id);
            }

            return View(model);
        }
        
        /// <summary>
        /// Removes a specified movie from the user's favorites
        /// </summary>
        /// <param name="movieId">The ID of the movie to remove</param>
        /// <returns>Updated user profile page</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int movieId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.Name).Value);

            bool success = await _userService.RemoveMovieFromFavorites(userId, movieId);
    
            if (success)
            {
                TempData["SuccessMessage"] = "Movie removed from favorites.";
            }
            else
            {
                TempData["ErrorMessage"] = "Could not remove the movie from favorites.";
            }
    
            return RedirectToAction("Profile");
        }
        
        /// <summary>
        /// Adds a comment to a specified movie by the logged-in user
        /// </summary>
        /// <param name="model">The comment model containing the movie ID and content of the comment</param>
        /// <returns>Updated movie details page</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment(CommentViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Redirect to login or handle the case when the user is not logged in
                return RedirectToAction("Login", "User");
            }

            // Retrieve the UserId claim
            var userIdClaim = User.FindFirst(ClaimTypes.Name);
            var usernameClaim = User.FindFirst("Username");

            // Check if the UserId claim exists and is not null
            if (userIdClaim == null || usernameClaim == null)
            {
                // Handle the case when the UserId or Username claim is not found
                // This could be a redirect or showing an error message
                return RedirectToAction("Index", "Home");
            }

            // Parse the UserId claim value to Guid
            var userId = Guid.Parse(userIdClaim.Value);

            if (ModelState.IsValid)
            {
                var comment = new Comment 
                {
                    MovieId = model.MovieId,
                    UserId = userId, 
                    Username = usernameClaim.Value,
                    Content = model.Content
                };

                await _userService.AddCommentAsync(comment);
            }

            return RedirectToAction("Details", "Home",new { id = model.MovieId });
        }

    }
}


