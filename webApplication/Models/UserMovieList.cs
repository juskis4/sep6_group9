using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApplication.Models
{
    [Table("user_movie_list", Schema = "public")]
    public class UserMovieList
    {
        [Key, Column(Order = 0)]
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Movie")]
        public int MovieId { get; set; }

        [Required]
        [Column(TypeName = "char(1)")]
        public char Type { get; set; } // Assuming 'F' or 'W' values

        public virtual User User { get; set; }
        public virtual Movie Movie { get; set; }
    }
}