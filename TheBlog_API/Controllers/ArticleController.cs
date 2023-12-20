using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using TheBlog_API.Auth;
using TheBlog_API.Data.Dtos;
using TheBlog_API.Data.Interfaces;
using TheBlog_API.Data.Models;
using TheBlog_API.Services.Interfaces;

namespace TheBlog_API.Controllers
{
    [Route("api/Articles")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet]
        public async Task<IEnumerable<RetrievedArticleDto>> GetAll()
        {
            var scheme = Request.Scheme;
            var host = Request.Host.ToString();
            var pathBase = Request.PathBase.ToString();

            var articles = await _articleService.GetAllArticles(scheme, host, pathBase);
            return articles;
        }

        [HttpGet]
        [Route("{articleId}", Name = "GetArticle")]
        public async Task<ActionResult<RetrievedArticleDto>> Get(int articleId)
        {
            var scheme = Request.Scheme;
            var host = Request.Host.ToString();
            var pathBase = Request.PathBase.ToString();

            var result = await _articleService.GetArticle(articleId, scheme, host, pathBase);
            return result;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ArticleDto>> Create([FromForm] CreateArticleDto createArticleDto)
        {
            var result = await _articleService.CreateArticle(createArticleDto, User);
            return result;
        }

        [Authorize]
        [HttpPatch]
        [Route("{articleId}")]
        public async Task<ActionResult<ArticleDto>> Update(int articleId, UpdateArticleDto updateArticleDto)
        {
            var result = await _articleService.UpdateArticle(articleId, updateArticleDto, User);
            return result;
        }

        [Authorize]
        [HttpDelete]
        [Route("{articleId}")]
        public async Task<ActionResult> Delete(int articleId)
        {
            var result = await _articleService.DeleteArticle(articleId, User);
            return result;
        }

        [HttpPost("{articleId}/upvote")]
        public async Task<IActionResult> UpvoteArticle(int articleId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var result = await _articleService.VoteArticle(articleId, userId, true);

            if (result != null)
            {
                return Ok(result);
            }

            return BadRequest(new { Message = "Failed to upvote the article." });
        }

        [HttpPost("{articleId}/downvote")]
        public async Task<IActionResult> DownvoteArticle(int articleId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var result = await _articleService.VoteArticle(articleId, userId, false);

            if (result != null)
            {
                return Ok(result);
            }

            return BadRequest(new { Message = "Failed to downvote the article." });
        }
    }
}
