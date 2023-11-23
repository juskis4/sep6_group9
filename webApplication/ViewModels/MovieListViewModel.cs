using System.Collections.Generic;

namespace webApplication.ViewModels
{
    public class MovieListViewModel
    {
        public IEnumerable<MovieViewModel> Movies { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        
        public string Search { get; set; }
    }
}