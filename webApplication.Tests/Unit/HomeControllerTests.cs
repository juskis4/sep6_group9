using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using webApplication.Controllers;
using webApplication.Services;
using webApplication.ViewModels;
using Xunit;

namespace webApplication.Tests.Unit
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _loggerMock;
        private readonly Mock<IMovieService> _movieServiceMock;
        private readonly Mock<IMovieDbService> _movieDbServiceMock;
        private readonly Mock<IUserService> _userServiceMock;

        public HomeControllerTests()
        {
            _loggerMock = new Mock<ILogger<HomeController>>();
            _movieServiceMock = new Mock<IMovieService>();
            _movieDbServiceMock = new Mock<IMovieDbService>();
            _userServiceMock = new Mock<IUserService>();
        }

        private HomeController CreateHomeController()
        {
            return new HomeController(_loggerMock.Object, _movieServiceMock.Object,
                _movieDbServiceMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task Index_ReturnsCorrectViewAndModel()
        {
            // Arrange
            var expectedMovies = new List<MovieViewModel>
            {
                new MovieViewModel 
                { 
                    Id = 8026966, 
                    Title = "Heartbreak Falls", 
                    Year = 2022, 
                    Rating = new RatingViewModel { MovieId = 1, DbValue = (float) 8.0, Votes = 150 }
                },
                new MovieViewModel 
                { 
                    Id = 13087500, 
                    Title = "Batman: Knightfall", 
                    Year = 2022, 
                    Rating = new RatingViewModel { MovieId = 2, DbValue = (float) 7.5, Votes = 2053 }
                }
            };

            _movieDbServiceMock.Setup(service => service.GetMoviesWithPagination(It.IsAny<int>(), It.IsAny<int>(), null, null))
                .ReturnsAsync(expectedMovies);
            _movieDbServiceMock.Setup(service => service.GetMovieCountAsync(null, null))
                .ReturnsAsync(expectedMovies.Count);

            var controller = CreateHomeController();

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<MovieListViewModel>(viewResult.Model);
            Assert.Equal(expectedMovies, model.Movies);
            Assert.Equal(1, model.CurrentPage); 
            Assert.Equal(1, model.TotalPages); 
        }

        [Fact]
        public async Task Search_WithValidQuery_ReturnsCorrectViewAndModel()
        {
            // Arrange
            var query = "test";
            var expectedMovies = new List<MovieViewModel>
            {
                new MovieViewModel 
                { 
                    Id = 12809652, 
                    Title = "Prime Farmland 2", 
                    Year = 2023, 
                    Rating = new RatingViewModel { MovieId = 12809652, DbValue = (float) 7.5, Votes = 200 }
                },
                new MovieViewModel 
                { 
                    Id = 12880350, 
                    Title = "Immortal Egypt", 
                    Year = 2023, 
                    Rating = new RatingViewModel { MovieId = 12880350, DbValue = (float) 8.0, Votes = 500 }
                }
            };

            _movieDbServiceMock.Setup(service => service.GetSearchedMoviesWithPaginationAsync(query, It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(expectedMovies);
            _movieDbServiceMock.Setup(service => service.GetSearchedMoviesCountAsync(query))
                .ReturnsAsync(expectedMovies.Count);

            var controller = CreateHomeController();

            // Act
            var result = await controller.Search(query);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<MovieListViewModel>(viewResult.Model);
            Assert.Equal(expectedMovies, model.Movies);
            Assert.Equal(query, model.Search);
            Assert.Equal(1, model.CurrentPage); 
            Assert.Equal(1, model.TotalPages); 
        }

        [Fact]
        public async Task Search_WithEmptyQuery_RedirectsToIndex()
        {
            // Arrange
            string emptyQuery = "";
            var controller = CreateHomeController();

            // Act
            var result = await controller.Search(emptyQuery);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Details_WithValidMovieId_ReturnsCorrectViewAndModel()
        {
            // Arrange
            int movieId = 35423;
            var expectedMovieViewModel = new MovieViewModel
            {
                Id = movieId,
                Title = "Kate & Leopold",
                Year = 2001,
                Rating = new RatingViewModel
                {
                    MovieId = movieId,
                    DbValue = (float) 8.5,
                    Votes = 1000
                },
                Stars = new List<PersonViewModel>
                {
                    new PersonViewModel {Id = 1, Name = "Actor 1", BirthYear = 1980},
                    new PersonViewModel {Id = 2, Name = "Actor 2", BirthYear = 1985}
                },
                Directors = new List<PersonViewModel>
                {
                    new PersonViewModel {Id = 3, Name = "Director 1", BirthYear = 1970}
                },
                Comments = new List<CommentViewModel>
                {
                    new CommentViewModel {CommentId = 1, Username = "User1", Content = "Great movie!"}
                }
            };

            _movieDbServiceMock.Setup(service => service.GetMovieAsync(movieId))
                .ReturnsAsync(expectedMovieViewModel);
            var controller = CreateHomeController();

            // Act
            var result = await controller.Details(movieId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<MovieViewModel>(viewResult.Model);
            Assert.Equal(expectedMovieViewModel.Id, model.Id);
            Assert.Equal(expectedMovieViewModel.Title, model.Title);
            Assert.Equal(expectedMovieViewModel.Year, model.Year);
            Assert.Equal(expectedMovieViewModel.Rating.DbValue, model.Rating.DbValue);
            Assert.Equal(expectedMovieViewModel.Rating.Votes, model.Rating.Votes);
            Assert.Equal(expectedMovieViewModel.Stars.Count, model.Stars.Count); 
            Assert.Equal(expectedMovieViewModel.Directors.Count, model.Directors.Count);
        }

        [Fact]
        public async Task Details_WithInvalidMovieId_ReturnsNotFound()
        {
            // Arrange
            int? invalidMovieId = null;
            _movieDbServiceMock.Setup(service => service.GetMovieAsync(It.IsAny<int>()))
                .ReturnsAsync((MovieViewModel?) null);
            var controller = CreateHomeController();

            // Act
            var result = await controller.Details(invalidMovieId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}