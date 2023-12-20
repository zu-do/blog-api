using System.ComponentModel.DataAnnotations;

namespace TheBlog_API.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool? IsBlocked {  get; set; }

        [Required]
        public int ArticleId { get; set; }
        public Article Article { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<Report> Reports { get; set; }
    }
}
