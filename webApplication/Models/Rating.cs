using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApplication.Models
{
    public class Rating
    {
        [Key]
        [ForeignKey("Movie")]
        [Range(0, 99999)]
        public int MovieId { get; set; }

        [Required]
        [Range(0.0, 10.0)]
        public double Ranking { get; set; }

        [Required]
        public int Votes { get; set; }
    }
}