using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApplication.Models
{
    [Table("users", Schema = "public")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("username")]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("password")]
        public string Password { get; set; }
        
        //for 1 to many relationship navigation
        public virtual ICollection<UserMovieList> UserFavoriteMovieList { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}