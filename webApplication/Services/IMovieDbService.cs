using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webApplication.Models;
using webApplication.ViewModels;

namespace webApplication.Services
{
    public interface IMovieDbService
    {
        Task<int> GetMovieCountAsync();

        Task<List<MovieViewModel>> GetMoviesWithPagination(int page, int PageSize = 12);
        
        Task<MovieViewModel> GetMovieAsync(int? id);

        Task<IQueryable<Movie>> GetSearchedMoviesAsync(string query);
    }
}