using Microsoft.AspNetCore.Identity;

namespace TheBlog_API.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool CanWriteArticles { get; set; } = false;
        public bool CanRankArticles { get; set; } = false;
        public bool CanWriteComments { get; set; } = false;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }

        public ICollection<UserArticleVote> ArticleVotes { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Report> ReportedComments { get; set; }
    }
}
