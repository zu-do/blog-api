using TheBlog_API.Data.Models;

namespace TheBlog_API.Data.Interfaces
{
    public interface ICommentRepository
    {
        Task CreateAsync(Comment comment);
        Task DeleteAsync(Comment comment);
        Task<IReadOnlyList<Comment>> GetAllAsync(int articleId);
        Task<IReadOnlyList<Comment>> GetAllReportedAsync();
        Task<Comment?> GetAsync(int articleId, int commentId);
        Task UpdateAsync(Comment comment);
    }
}