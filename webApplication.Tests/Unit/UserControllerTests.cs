using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using webApplication.Controllers;
using webApplication.Models;
using webApplication.Services;
using webApplication.ViewModels;
using Xunit;
using IAuthenticationService = webApplication.Services.IAuthenticationService;

namespace webApplication.Tests.Unit
{
    public class UserControllerTests
    {
        private UserController SetupController(IUserService userService, IMovieService movieService, IAuthenticationService authService)
        {
            return new UserController(userService, movieService, authService);
        }
        
        private static void MockHttpContext(ControllerBase controller)
        {
            var context = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
        }
        
        [Fact]
        public void Login_Get_ReturnsViewResult()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var movieServiceMock = new Mock<IMovieService>();
            var authServiceMock = new Mock<IAuthenticationService>();
            var controller = SetupController(userServiceMock.Object, movieServiceMock.Object, authServiceMock.Object);

            // Act
            var result = controller.Login();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True((bool)(viewResult.ViewData["HideNavBar"] ?? throw new InvalidOperationException()));
        }
        
        [Fact]
        public async Task Login_Post_ValidModel_RedirectsToHomeIndex()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var movieServiceMock = new Mock<IMovieService>();
            var authServiceMock = new Mock<IAuthenticationService>();
    
            var userViewModel = new UserViewModel
            {
                Username = "testUser",
                Password = "testPassword"
            };

            userServiceMock.Setup(s => s.ValidateUserAsync("testUser", "testPassword"))
                .ReturnsAsync(new User { UserId = Guid.NewGuid(), Username = "testUser" });

            var controller = SetupController(userServiceMock.Object, movieServiceMock.Object, authServiceMock.Object);

            // Act
            var result = await controller.Login(userViewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
            userServiceMock.Verify(s => s.ValidateUserAsync("testUser", "testPassword"), Times.Once);
            authServiceMock.Verify(a => a.SignInUserAsync(It.IsAny<User>(), It.IsAny<HttpContext>()), Times.Once);
        }
        
        [Fact]
        public async Task Register_Post_ValidModel_RedirectsToLogin()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var movieServiceMock = new Mock<IMovieService>();
            var authServiceMock = new Mock<IAuthenticationService>();
            var controller = SetupController(userServiceMock.Object, movieServiceMock.Object, authServiceMock.Object);
            MockHttpContext(controller);

            var registerViewModel = new RegisterViewModel
            {
                Username = "newUser",
                Password = "password",
                ConfirmPassword = "password"
            };

            userServiceMock.Setup(s => s.VerifyUser(It.IsAny<string>()))
                .ReturnsAsync(false);
            userServiceMock.Setup(s => s.RegisterUser(It.IsAny<RegisterViewModel>()))
                .ReturnsAsync(true);

            // Act
            var result = await controller.Register(registerViewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
            Assert.Equal("User", redirectResult.ControllerName);
        }
        
        [Fact]
        public async Task Profile_Get_AuthenticatedUser_ReturnsViewWithModel()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var movieServiceMock = new Mock<IMovieService>();
            var authServiceMock = new Mock<IAuthenticationService>();
            var controller = SetupController(userServiceMock.Object, movieServiceMock.Object, authServiceMock.Object);
            MockHttpContext(controller);

            var guid = Guid.NewGuid();
            controller.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, guid.ToString())
            }, CookieAuthenticationDefaults.AuthenticationScheme));

            var movies = new List<MovieViewModel>(); 
            if (movies == null) throw new ArgumentNullException(nameof(movies));
            userServiceMock.Setup(s => s.GetFavoriteMovies(It.IsAny<Guid>()))
                .ReturnsAsync(movies);

            // Act
            var result = await controller.Profile();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<MovieListViewModel>(viewResult.Model);
            Assert.NotNull(model.Movies);
        }
    }
}
