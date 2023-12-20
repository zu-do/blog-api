using Microsoft.EntityFrameworkCore;
using TheBlog_API.Data.Interfaces;
using TheBlog_API.Data.Models;

namespace TheBlog_API.Data.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly BlogDbContext _dbContext;

        public ReportRepository(BlogDbContext context)
        {
            _dbContext = context;
        }

        public async Task<Report?> GetAsync(int commentId, string userId)
        {
            return await _dbContext.DbReports.FirstOrDefaultAsync(r => r.CommentId == commentId && r.ApplicationUserId == userId);
        }

        public async Task CreateAsync(Report report)
        {
            _dbContext.DbReports.Add(report);
            await _dbContext.SaveChangesAsync();
        }
    }
}
