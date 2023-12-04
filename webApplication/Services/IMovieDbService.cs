using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webApplication.Models;
using webApplication.ViewModels;

namespace webApplication.Services
{
    public interface IMovieDbService
    {
        Task<int> GetMovieCountAsync(int? year = null, double? minRating = null);

        Task<IEnumerable<MovieViewModel>> GetMoviesWithPagination(int page, int PageSize = 12, int? year = null, double? minRating = null);
        
        Task<MovieViewModel> GetMovieAsync(int? id);

        Task<IQueryable<Movie>> GetSearchedMoviesAsync(string query);

        Task<List<int?>> GetMovieYears();

    }
}