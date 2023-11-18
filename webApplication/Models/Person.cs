using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace webApplication.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        public int? Birth { get; set; }

        // Navigation properties
        public virtual ICollection<Star> StarredMovies { get; set; } = new HashSet<Star>();
        public virtual ICollection<Director> DirectedMovies { get; set; } = new HashSet<Director>();
    }
}