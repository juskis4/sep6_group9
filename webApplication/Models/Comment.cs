using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApplication.Models
{
    [Table("comments", Schema = "public")]
    public class Comment
    {
        [Key]
        [Column("comment_id")]
        public int CommentId { get; set; }

        [Required]
        [ForeignKey("Movie")]
        [Column("movie_id")]
        public int MovieId { get; set; }

        [Required]
        [ForeignKey("User")]
        [Column("user_id")]
        public Guid UserId { get; set; }
        
        [Required]
        [MaxLength(50)]
        [Column("username")]
        public string Username { get; set; }
        
        [Required]
        [StringLength(1000, ErrorMessage = "Comment cannot be longer than 1000 characters.")]
        [Column("content")]
        public string Content { get; set; }

        // Navigation properties
        public virtual Movie Movie { get; set; }
        public virtual User User { get; set; }
    }
}