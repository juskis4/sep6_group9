using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApplication.Models
{
    [Table("people", Schema = "public")]
    public class Person
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }
        
        [Column("birth")]
        public int? Birth { get; set; }

        // Navigation properties
        public virtual ICollection<Star> StarredMovies { get; set; } = new HashSet<Star>();
        public virtual ICollection<Director> DirectedMovies { get; set; } = new HashSet<Director>();
    }
}