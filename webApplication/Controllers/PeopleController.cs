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
    }
}