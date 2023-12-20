using System.ComponentModel.DataAnnotations;

namespace TheBlog_API.Data.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? ImageName { get; set; }
        public string? Text { get; set; }

        public int? Rank { get; set; }

        [Required] 
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<UserArticleVote> UserVotes { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
