using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApplication.Models
{
    public class Rating
    {
        [Key]
        [ForeignKey("Movie")]
        public int MovieId { get; set; }

        [Required]
        public float RatingValue { get; set; }

        [Required]
        public int Votes { get; set; }

        // Navigation property
        public virtual Movie Movie { get; set; }
    }
}