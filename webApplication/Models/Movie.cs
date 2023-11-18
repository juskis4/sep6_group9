using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace webApplication.Models
{
    public class Movie
    {
        [Key]
        [Range(0, 99999)]
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        [Range(0, 9999)]
        public int Year { get; set; }
        
        public Rating Rating { get; set; }
        
        public List<Star> Stars { get; set; }

        public List<Director> Directors { get; set; }
    }
}