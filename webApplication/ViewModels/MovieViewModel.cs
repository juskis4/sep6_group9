using System.Collections.Generic;

namespace webApplication.ViewModels
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public RatingViewModel Rating { get; set; }
        public List<PersonViewModel> Stars { get; set; }
        public List<PersonViewModel> Directors { get; set; }
    }
}