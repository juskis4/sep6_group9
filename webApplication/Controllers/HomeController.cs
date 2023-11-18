using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using webApplication.Data;
using webApplication.Models;

namespace webApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MovieDataContext _context;

        public HomeController(ILogger<HomeController> logger, MovieDataContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            //var movies = this._context.Movies.Include("Rating");
            try
            {
                var canConnect = _context.Database.CanConnect();
                if (canConnect)
                {
                    Console.WriteLine("Connection successful!");
                    var moviesCount = _context.Movies.Count();
                    Console.WriteLine($"There are {moviesCount} movies.");
                }
                else
                {
                    Console.WriteLine("Unable to connect to the database.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to connect to the database: {ex.Message}");
            }

            //NOT good to return whole dataset, remove IDs and such first
            return View(null);
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