using TheBlog_API.Data.Models;

namespace TheBlog_API.Data.Interfaces
{
    public interface IArticleRepository
    {
        Task CreateAsync(Article article);
        Task DeleteAsync(Article article);
        Task<IReadOnlyList<Article>> GetAllAsync();
        Task<Article?> GetAsync(int articleId);
        Task UpdateAsync(Article article);
    }
}