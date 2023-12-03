using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webApplication.Models;
using webApplication.Services;
using webApplication.ViewModels;

namespace webApplication.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            ViewBag.HideNavBar = true;
            return View();
        }

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
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.HideNavBar = true;
            return View();
        }

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
    }
}


