using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webApplication.Services;

namespace webApplication.Controllers
{
    /// <summary>
    /// Controller for managing and displaying information about people involved in movies, such as stars and directors
    /// </summary>
    [Authorize]
    public class PeopleController : Controller
    {
        private readonly IMovieDbService _movieDbService;

        /// <summary>
        /// Initializes a new instance of the PeopleController class
        /// </summary>
        /// <param name="movieDbService">Service for movie database operations</param>
        public PeopleController(IMovieDbService movieDbService)
        {
            _movieDbService = movieDbService;
        }

        /// <summary>
        /// Displays a paginated list of stars
        /// </summary>
        /// <param name="page">Current page number for pagination</param>
        /// <param name="PageSize">Number of stars per page</param>
        /// <returns>Stars View</returns>
        [HttpGet]
        public async Task<IActionResult> Stars(int page, int PageSize = 30)
        {
            var totalStarsCount = await _movieDbService.GetStarsCountAsync();
            var totalPages = (int) Math.Ceiling(totalStarsCount / (double) PageSize);

            page = Math.Max(1, Math.Min(page, totalPages));
            
            var stars = await _movieDbService.GetStarsWithPaginationAsync(page, PageSize);
            
            stars.CurrentPage = page;
            stars.TotalPages = totalPages;
            
            return View("~/Views/Home/Stars.cshtml", stars);
        }
        
        /// <summary>
        /// Displays a paginated list of directors
        /// </summary>
        /// <param name="page">Current page number for pagination</param>
        /// <param name="PageSize">Number of directors per page</param>
        /// <returns>Directors View</returns>
        [HttpGet]
        public async Task<IActionResult> Directors(int page, int PageSize = 30)
        {
            var totalDirectorsCount = await _movieDbService.GetDirectorsCountAsync();
            var totalPages = (int) Math.Ceiling(totalDirectorsCount / (double) PageSize);

            page = Math.Max(1, Math.Min(page, totalPages));
            
            var directors = await _movieDbService.GetDirectorsWithPaginationAsync(page, PageSize);
            
            directors.CurrentPage = page;
            directors.TotalPages = totalPages;
            
            return View("~/Views/Home/Directors.cshtml", directors);
        }
    }
}