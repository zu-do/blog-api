using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheBlog_API.Data.Dtos;

namespace TheBlog_API.Services.Interfaces
{
    public interface IArticleService
    {
        Task<ActionResult<ArticleDto>> CreateArticle(CreateArticleDto createArticleDto, ClaimsPrincipal user);
        Task<ActionResult> DeleteArticle(int articleId, ClaimsPrincipal user);
        Task<IEnumerable<RetrievedArticleDto>> GetAllArticles(string scheme, string host, string pathBase);
        Task<ActionResult<RetrievedArticleDto>> GetArticle(int articleId, string scheme, string host, string pathBase);
        Task<ActionResult<ArticleDto>> UpdateArticle(int articleId, UpdateArticleDto updateArticleDto, ClaimsPrincipal user);
        Task UpdateArticleRank(int articleId);
        Task<int?> VoteArticle(int articleId, string userId, bool isUpVote);
    }
}