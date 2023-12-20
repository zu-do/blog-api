using TheBlog_API.Data.Models;

namespace TheBlog_API.Data.Interfaces
{
    public interface IReportRepository
    {
        Task CreateAsync(Report report);
        Task<Report?> GetAsync(int commentId, string userId);
    }
}