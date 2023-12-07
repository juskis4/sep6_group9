using System;

namespace webApplication.ViewModels
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }
        
        public int MovieId { get; set; }
        
        public Guid UserId { get; set; }
        
        public string Username { get; set; }
        
        public string Content { get; set; }
    }
}