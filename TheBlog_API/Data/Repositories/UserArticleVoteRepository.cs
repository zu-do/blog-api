using Microsoft.EntityFrameworkCore;
using TheBlog_API.Data.Interfaces;
using TheBlog_API.Data.Models;

namespace TheBlog_API.Data.Repositories
{
    public class UserArticleVoteRepository : IUserArticleVoteRepository
    {
        private readonly BlogDbContext _dbContext;

        public UserArticleVoteRepository(BlogDbContext context)
        {
            _dbContext = context;
        }

        public async Task<UserArticleVote?> GetAsync(int articleId, string userId)
        {
            return await _dbContext.DbUserArticleVotes.FirstOrDefaultAsync(uav => uav.ArticleId == articleId && uav.UserId == userId);
        }

        public async Task CreateAsync(UserArticleVote userArticleVote)
        {
            _dbContext.DbUserArticleVotes.Add(userArticleVote);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserArticleVote userArticleVote)
        {
            _dbContext.DbUserArticleVotes.Update(userArticleVote);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CountArticleVotes(int articleId, bool isUpVote)
        {
            return await _dbContext.DbUserArticleVotes.CountAsync(uav => uav.ArticleId == articleId && uav.IsUpVote == isUpVote);
        }
    }
}
