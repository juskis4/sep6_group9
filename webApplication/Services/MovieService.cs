using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using webApplication.ViewModels;

namespace webApplication.Services
{
    public class MovieService : IMovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "";
        //delete when commiting + don't commit appsettings.json 220d1d34

        public MovieService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<MovieDetailsViewModel> GetMovieDetailsAsync(int movieId)
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
