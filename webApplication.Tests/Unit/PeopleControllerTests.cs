using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using webApplication.Controllers;
using webApplication.Services;
using webApplication.ViewModels;
using Xunit;

namespace webApplication.Tests.Unit
{
    public class PeopleControllerTests
    {
        private PeopleController SetupController(IMovieDbService movieDbService)
        {
            return new PeopleController(movieDbService);
        }

        [Fact]
        public async Task Stars_ReturnsCorrectViewAndModel()
        {
            // Arrange
            var movieDbServiceMock = new Mock<IMovieDbService>();
            var starsViewModel = new PersonListViewModel(); 
            const int totalStarsCount = 100; 
            const int page = 1;
            const int pageSize = 30;

            movieDbServiceMock.Setup(s => s.GetStarsCountAsync()).ReturnsAsync(totalStarsCount);
            movieDbServiceMock.Setup(s => s.GetStarsWithPaginationAsync(page, pageSize)).ReturnsAsync(starsViewModel);

            var controller = SetupController(movieDbServiceMock.Object);

            // Act
            var result = await controller.Stars(page, pageSize);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Home/Stars.cshtml", viewResult.ViewName);
            Assert.Equal(starsViewModel, viewResult.Model);
            movieDbServiceMock.Verify(s => s.GetStarsCountAsync(), Times.Once);
            movieDbServiceMock.Verify(s => s.GetStarsWithPaginationAsync(page, pageSize), Times.Once);
        }

        [Fact]
        public async Task Directors_ReturnsCorrectViewAndModel()
        {
            // Arrange
            var movieDbServiceMock = new Mock<IMovieDbService>();
            var directorsViewModel = new PersonListViewModel();
            const int totalDirectorsCount = 100; 
            const int page = 1;
            const int pageSize = 30;

            movieDbServiceMock.Setup(s => s.GetDirectorsCountAsync()).ReturnsAsync(totalDirectorsCount);
            movieDbServiceMock.Setup(s => s.GetDirectorsWithPaginationAsync(page, pageSize))
                .ReturnsAsync(directorsViewModel);

            var controller = SetupController(movieDbServiceMock.Object);

            // Act
            var result = await controller.Directors(page, pageSize);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Home/Directors.cshtml", viewResult.ViewName);
            Assert.Equal(directorsViewModel, viewResult.Model);
            movieDbServiceMock.Verify(s => s.GetDirectorsCountAsync(), Times.Once);
            movieDbServiceMock.Verify(s => s.GetDirectorsWithPaginationAsync(page, pageSize), Times.Once);
        }
    }
}