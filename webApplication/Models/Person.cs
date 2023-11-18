using System.ComponentModel.DataAnnotations;

namespace webApplication.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required]
        [Range(0, 9999)]
        public int Birth { get; set; }
    }
}