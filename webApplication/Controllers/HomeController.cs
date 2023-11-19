using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using webApplication.Data;
using webApplication.Models;
using webApplication.ViewModels;

namespace webApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MovieDataContext _context;

        private const int PageSize = 12;

        public HomeController(ILogger<HomeController> logger, MovieDataContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            var totalMoviesCount = await _context.Movies.CountAsync();
            var totalPages = (int) Math.Ceiling(totalMoviesCount / (double) PageSize);

            // Ensure the current page isn't out of range
            page = Math.Max(1, Math.Min(page, totalPages));

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
                                Value = m.Rating.RatingValue,
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
                TotalPages = totalPages
            };

            return View(model);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}