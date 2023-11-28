using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using webApplication.Data;
using webApplication.Interfaces;
using webApplication.Models;
using webApplication.Services;
using Xunit;

public class MovieServiceTests
{
    private readonly Mock<IHttpClientWrapper> _httpClientMock;
    private readonly Mock<IMovieDataContext> _dbContextMock;
    private readonly Mock<DbSet<Movie>> _mockSet;
    private readonly MovieService _service;

    public MovieServiceTests()
    {
        _httpClientMock = new Mock<IHttpClientWrapper>();
        _dbContextMock = new Mock<IMovieDataContext>();

        _mockSet = new Mock<DbSet<Movie>>();
        var data = new List<Movie>
        {
            new Movie
            {
                Id = 15414,
                Title = "La tierra de los toros",
                Year = 2000,
                Rating = new Rating
                {
                    MovieId = 15414,
                    RatingValue = (float) 5.4,
                    Votes = 12
                },
                Stars = new HashSet<Star>()
                {
                },
                Directors = new HashSet<Director>()
                {
                }
            },
            new Movie
            {
                Id = 15724,
                Title = "Dama de noche",
                Year = 1993,
                Rating = new Rating
                {
                    MovieId = 15724,
                    RatingValue = (float) 6.2,
                    Votes = 20
                },
                Stars = new HashSet<Star>()
                {
                },
                Directors = new HashSet<Director>()
                {
                }
            },
        }.AsQueryable();

        _mockSet.As<IQueryable<Movie>>().Setup(m => m.Provider).Returns(data.Provider);
        _mockSet.As<IQueryable<Movie>>().Setup(m => m.Expression).Returns(data.Expression);
        _mockSet.As<IQueryable<Movie>>().Setup(m => m.ElementType).Returns(data.ElementType);
        _mockSet.As<IQueryable<Movie>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        _dbContextMock.Setup(c => c.Movies).Returns(_mockSet.Object);

        _service = new MovieService(_httpClientMock.Object, _dbContextMock.Object);
    }

    [Fact]
    public async Task GetMovieDetailsAsync_ValidMovieId_ReturnsMovieDetails()
    {
        var movieId = 13229946;
        var expectedUrl = $"http://www.omdbapi.com/?i=tt{movieId}&plot=full&apikey=220d1d34";
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{\"Title\":\"Test Movie\",\"Year\":\"2021\"}")
        };

        _httpClientMock.Setup(client => client.GetAsync(It.Is<string>(url => url == expectedUrl)))
            .ReturnsAsync(httpResponseMessage);

        var result = await _service.GetMovieDetailsAsync(movieId);

        Assert.NotNull(result);
        Assert.Equal("Test Movie", result.Title);
        Assert.Equal("2021", result.Year);
    }

    [Fact]
    public async Task GetMovieDetailsAsync_InvalidMovieId_ThrowsException()
    {
        var movieId = 13229946;
        var expectedUrl = $"http://www.omdbapi.com/?i=tt{movieId}&plot=full&apikey=220d1d34";
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound
        };

        _httpClientMock.Setup(client => client.GetAsync(It.Is<string>(url => url == expectedUrl)))
            .ReturnsAsync(httpResponseMessage);

        await Assert.ThrowsAsync<Exception>(() => _service.GetMovieDetailsAsync(movieId));
    }

    [Fact]
    public async Task GetSearchResultsAsync_EmptyQuery_ThrowsArgumentException()
    {
        var query = "";
        var page = 1;

        await Assert.ThrowsAsync<ArgumentException>(() => _service.GetSearchResultsAsync(query, page));
    }

    [Fact]
    public async Task GetMoviePosterAsync_ErrorResponse_ThrowsException()
    {
        int movieId = 13229946;
        string apiKey = "220d1d34";
        string apiUrl = $"http://www.omdbapi.com/?i=tt{movieId}&plot=full&apikey={apiKey}";

        var httpClientWrapperMock = new Mock<IHttpClientWrapper>();
        httpClientWrapperMock
            .Setup(client => client.GetAsync(apiUrl))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        var movieService = new MovieService(httpClientWrapperMock.Object, _dbContextMock.Object);

        var exception = await Assert.ThrowsAsync<Exception>(() => movieService.GetMoviePosterAsync(movieId));

        Assert.Equal("Error retrieving movie details.", exception.Message);
    }
}