using TheBlog_API.Data.Models;

namespace TheBlog_API.Data.Interfaces
{
    public interface IUserArticleVoteRepository
    {
        Task<int> CountArticleVotes(int articleId, bool isUpVote);
        Task CreateAsync(UserArticleVote userArticleVote);
        Task<UserArticleVote?> GetAsync(int articleId, string userId);
        Task UpdateAsync(UserArticleVote userArticleVote);
    }
}