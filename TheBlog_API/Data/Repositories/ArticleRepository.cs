using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TheBlog_API.Data.Interfaces;
using TheBlog_API.Data.Models;

namespace TheBlog_API.Data.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly BlogDbContext _dbContext;

        public ArticleRepository(BlogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Article?> GetAsync(int articleId)
        {
            return await _dbContext.DbArticles
                .Include(a => a.User)
                .Include(a => a.Comments)
                .FirstOrDefaultAsync(a => a.Id == articleId);
        }

        public async Task<IReadOnlyList<Article>> GetAllAsync()
        {
            return await _dbContext.DbArticles
                .Include(a => a.User)
                .Include(a => a.Comments)
                .ToListAsync();
        }

        public async Task CreateAsync(Article article)
        {
            _dbContext.DbArticles.Add(article);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Article article)
        {
            _dbContext.DbArticles.Update(article);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Article article)
        {
            var userVotes = await _dbContext.DbUserArticleVotes
                .Where(uv => uv.ArticleId == article.Id)
                .ToListAsync();

            _dbContext.DbUserArticleVotes.RemoveRange(userVotes);

            var articleComments = await _dbContext.DbComments
                .Where(c => c.ArticleId == article.Id)
                .ToListAsync();

            foreach (var comment in articleComments)
            {
                var commentReports = await _dbContext.DbReports
                .Where(r => r.CommentId == comment.Id)
                .ToListAsync();

                _dbContext.DbReports.RemoveRange(commentReports);
            }

            _dbContext.DbComments.RemoveRange(articleComments);

            _dbContext.DbArticles.Remove(article);
            await _dbContext.SaveChangesAsync();
        }

    }
}
