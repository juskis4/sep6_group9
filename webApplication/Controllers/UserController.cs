using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using webApplication.Data;
using webApplication.Models;
using webApplication.ViewModels;

namespace webApplication.Controllers
{
    public class UserController : Controller
    {
        private readonly UserDataContext _dbContext;
        
        

       public IActionResult Lists()
        {
            // Add logic here to fetch and display user-specific lists of movies.
            return View();
        }

        // Add more actions for user-related functionality as needed.

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LogInViewModel model)
        {
            if (IsValidUser(model.Username, model.Password))
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View(model);
        }
        private bool IsValidUser(string username, string password)
        {
            var user = _dbContext.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password);
            return user != null;
        }
        [HttpGet]
        public IActionResult Register()
        {
            // Render the registration form
            return View();
        }
        
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the username is already taken
                if (_dbContext.Users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Username is already taken.");
                    return View(model);
                }
                
                var newUser = new User()
                {
                    Username = model.Username,
                    Password = model.Password,
                    //do we hash the password? or is it enough like this?

                };

                _dbContext.Users.Add(newUser);
                _dbContext.SaveChanges();

                return RedirectToAction("Login");
            }
            
            return View(model);
        }
    }
}