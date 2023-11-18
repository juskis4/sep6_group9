using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApplication.Models
{
    public class Director
    {
        [Key]
        [ForeignKey("Movie")]
        [Range(0, 99999)]
        public int MovieId { get; set; }
        
        [ForeignKey("Person")]
        public int PersonId { get; set; }
        
        public Person Person { get; set; }
    }
}