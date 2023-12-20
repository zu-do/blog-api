using System.ComponentModel.DataAnnotations;

namespace TheBlog_API.Data.Models
{
    public class UserArticleVote
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public int ArticleId { get; set; }
        public Article Article { get; set; }

        [Required]
        public bool IsUpVote { get; set; }
    }
}
