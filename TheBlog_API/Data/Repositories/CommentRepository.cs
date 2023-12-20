using Microsoft.EntityFrameworkCore;
using TheBlog_API.Data.Interfaces;
using TheBlog_API.Data.Models;

namespace TheBlog_API.Data.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly BlogDbContext _dbContext;

        public CommentRepository(BlogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Comment?> GetAsync(int articleId, int commentId)
        {
            return await _dbContext.DbComments
                .Include(c => c.User)
                .Include(c => c.Reports)
                .FirstOrDefaultAsync(c => c.ArticleId == articleId && c.Id == commentId);
        }

        public async Task<IReadOnlyList<Comment>> GetAllAsync(int articleId)
        {
            return await _dbContext.DbComments
                .Where(c => c.ArticleId == articleId)
                .Include(c => c.User)
                .Include(c => c.Reports)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Comment>> GetAllReportedAsync()
        {
            return await _dbContext.DbComments
                .Where(c => c.Reports.Count > 0)
                .Include(c => c.User)
                .Include(c => c.Reports)
                .ToListAsync();
        }

        public async Task CreateAsync(Comment comment)
        {
            _dbContext.DbComments.Add(comment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Comment comment)
        {
            _dbContext.DbComments.Update(comment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Comment comment)
        {
            _dbContext.DbComments.Remove(comment);
            await _dbContext.SaveChangesAsync();
        }
    }
}
