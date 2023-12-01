using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApplication.Models
{
    [Table("movies", Schema = "public")]
    public class Movie
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("title")]
        public string Title { get; set; }
        
        [Column("year")]
        public int? Year { get; set; }

        // Navigation properties
        public virtual ICollection<Star> Stars { get; set; } = new HashSet<Star>();
        public virtual ICollection<Director> Directors { get; set; } = new HashSet<Director>();
        public virtual Rating Rating { get; set; }
        
        public virtual ICollection<UserMovieList> UserFavoriteMovieList { get; set; }
    }
}