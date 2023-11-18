using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApplication.Models
{
    [Table("movies", Schema = "public")]
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        
        public int? Year { get; set; }

        // Navigation properties
        public virtual ICollection<Star> Stars { get; set; } = new HashSet<Star>();
        public virtual ICollection<Director> Directors { get; set; } = new HashSet<Director>();
        public virtual Rating Rating { get; set; }
    }
}