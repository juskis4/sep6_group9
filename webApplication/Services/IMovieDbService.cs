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

        
        //Search
        Task<IEnumerable<MovieViewModel>> GetSearchedMoviesWithPaginationAsync(string query, int page, int PageSize = 12);

        Task<int> GetSearchedMoviesCountAsync(string query);
        

        Task<List<int?>> GetMovieYears();

        Task<IEnumerable<Comment>> GetCommentsByMovieIdAsync(int movieId);
        
        Task<PersonListViewModel> GetStarsWithPaginationAsync(int page, int PageSize = 12); 
            
        Task<int> GetStarsCountAsync();

        Task<PersonListViewModel> GetDirectorsWithPaginationAsync(int page, int pageSize = 12);

        Task<int> GetDirectorsCountAsync();


    }
}