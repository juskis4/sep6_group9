using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApplication.Models
{
    [Table("user_movie_list", Schema = "public")]
    public class UserMovieList
    {
        [Key]
        [ForeignKey("User")]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Key]
        [ForeignKey("Movie")]
        [Column("movie_id")]
        public int MovieId { get; set; }

        [Required]
        [Column("type")]
        public char Type { get; set; } // Assuming 'F' or 'W' values

        public virtual User User { get; set; }
        public virtual Movie Movie { get; set; }
    }
}