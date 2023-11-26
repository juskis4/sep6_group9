namespace webApplication.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [ForeignKey("Users")]
    [Column("user_id")]
    public int UserId { get; set; }
    
    [Required]
    [Column("username")]
    public string Username { get; set; }
    
    [Required]
    [Column("password")]
    public string Password { get; set; }
}