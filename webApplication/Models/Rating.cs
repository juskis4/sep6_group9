using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApplication.Models
{
    [Table("ratings", Schema = "public")]
    public class Rating
    {
        [Key]
        [ForeignKey("Movie")]
        [Column("movie_id")]
        public int MovieId { get; set; }

        [Required]
        [Column("rating")]
        public float RatingValue { get; set; }

        [Required]
        [Column("votes")]
        public int Votes { get; set; }

        // Navigation property
        public virtual Movie Movie { get; set; }
    }
}