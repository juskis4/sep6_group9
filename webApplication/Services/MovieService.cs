using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using webApplication.ViewModels;

namespace webApplication.Services
{
    public class MovieService : IMovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;

        public MovieService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = Environment.GetEnvironmentVariable("OMDbApi"); 
            
            //Used to initialize _apiKey when working locally 
            //configuration["OMDbApi:ApiKey"];
        }

        public async Task<MovieDetailsViewModel?> GetMovieDetailsAsync(int movieId)
        {
            string apiUrl = $"http://www.omdbapi.com/?i=tt{movieId}&plot=full&apikey={_apiKey}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<MovieDetailsViewModel>(content);
            }

            throw new Exception("Error retrieving movie details.");
        }

        public async Task<string?> GetMoviePosterAsync(int movieId)
        {
            string apiUrl = $"http://www.omdbapi.com/?i=tt{movieId}&plot=full&apikey={_apiKey}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<MovieDetailsViewModel>(content)?.Poster;
            }

            throw new Exception("Error retrieving movie details.");
        }
    }
}

   
    
