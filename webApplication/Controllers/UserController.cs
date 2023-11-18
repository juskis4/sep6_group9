using Microsoft.AspNetCore.Mvc;

namespace webApplication.Controllers
{
    public class UserController : Controller
    {
        public UserController()
        {
            
        }

        public IActionResult Lists()
        {
            // Add logic here to fetch and display user-specific lists of movies.
            return View();
        }

        // Add more actions for user-related functionality as needed.
    }
}
