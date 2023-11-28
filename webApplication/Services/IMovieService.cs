using System.Threading.Tasks;
using webApplication.ViewModels;

namespace webApplication.Services
{
    public interface IMovieService
    {
        Task<MovieDetailsViewModel> GetMovieDetailsAsync(int movieId);
        Task<string?> GetMoviePosterAsync(int movieId);

        Task<MovieListViewModel> GetSearchResultsAsync(string query, int page);
    }
}