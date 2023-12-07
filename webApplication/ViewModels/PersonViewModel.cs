using System.Collections.Generic;

namespace webApplication.ViewModels
{
    public class PersonViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? BirthYear { get; set; }
        
        public List<MovieViewModel> Movies { get; set; }
    }
}