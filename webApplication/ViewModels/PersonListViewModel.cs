using System.Collections.Generic;

namespace webApplication.ViewModels
{
    public class PersonListViewModel
    {
        public IEnumerable<PersonViewModel> People { get; set; }
        
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}