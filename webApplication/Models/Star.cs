using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApplication.Models
{
    public class Star
    {
        [ForeignKey("Movie")]
        public int MovieId { get; set; }
        
        [ForeignKey("Person")]
        public int PersonId { get; set; }

        // Navigation properties
        public virtual Movie Movie { get; set; }
        public virtual Person Person { get; set; }
    }
}