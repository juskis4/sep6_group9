using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webApplication.Services;

namespace webApplication.Controllers
{
    [Authorize]
    public class PeopleController : Controller
    {
        private readonly IMovieDbService _movieDbService;

        public PeopleController(IMovieDbService movieDbService)
        {
            _movieDbService = movieDbService;
        }

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