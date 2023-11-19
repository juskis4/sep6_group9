using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApplication.Models
{
    [Table("directors", Schema = "public")]
    public class Director
    {
        [ForeignKey("Movie")]
        [Column("movie_id")]
        public int MovieId { get; set; }
        
        [ForeignKey("Person")]
        [Column("person_id")]
        public int PersonId { get; set; }

        // Navigation properties
        public virtual Movie Movie { get; set; }
        public virtual Person Person { get; set; }
    }
}