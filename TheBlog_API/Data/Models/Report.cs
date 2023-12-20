using System.ComponentModel.DataAnnotations;

namespace TheBlog_API.Data.Models
{
    public class Report
    {
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        [Required]
        public int CommentId { get; set; }
    }
}
