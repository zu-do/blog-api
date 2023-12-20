using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using TheBlog_API.Data.Dtos;
using TheBlog_API.Data.Interfaces;
using TheBlog_API.Data.Models;
using TheBlog_API.Services.Interfaces;
using System.Linq;
using TheBlog_API.Auth;

namespace TheBlog_API.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IUserArticleVoteRepository _userArticleVoteRepository;
        private readonly IAuthService _authService;
        private readonly IImageService _imageService;

        public ArticleService(IArticleRepository articleRepository, IUserArticleVoteRepository userArticleVoteRepository, IAuthService authService, IImageService imageService)
        {
            _articleRepository = articleRepository;
            _userArticleVoteRepository = userArticleVoteRepository;
            _authService = authService;
            _imageService = imageService;
        }

        public async Task<IEnumerable<RetrievedArticleDto>> GetAllArticles(string scheme, string host, string pathBase)
        {
            var articles = await _articleRepository.GetAllAsync();

            return articles.Select(a => new RetrievedArticleDto(
                a.Id,
                a.Title,
                a.UserId,
                a.User.UserName,
                a.Rank,
                a.ImageName,
                a.Text,
                string.Format("{0}://{1}{2}/Images/{3}", scheme, host, pathBase, a.ImageName),
                a.Comments?.Any() == true ? a.Comments.Max(c => c.CreatedAt) : (DateTime?)null
            ));
        }

        public async Task<ActionResult<RetrievedArticleDto>> GetArticle(int articleId, string scheme, string host, string pathBase)
        {
            var article = await _articleRepository.GetAsync(articleId);

            if (article == null)
            {
                return new NotFoundResult();
            }

            var retrievedArticle = new RetrievedArticleDto(
                article.Id,
                article.Title,
                article.UserId,
                article.User.UserName,
                article.Rank,
                article.ImageName,
                article.Text,
                string.Format("{0}://{1}{2}/Images/{3}", scheme, host, pathBase, article.ImageName),
                article.Comments?.Any() == true ? article.Comments.Max(c => c.CreatedAt) : (DateTime?)null
            );

            return new ActionResult<RetrievedArticleDto>(retrievedArticle);
        }

        public async Task<ActionResult<ArticleDto>> CreateArticle(CreateArticleDto createArticleDto, ClaimsPrincipal user)
        {
            var id = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var currentUser = await _authService.GetUserById(id);

            if (!currentUser.CanWriteArticles)
            {
                return new ForbidResult();
            }

            var article = new Article
            {
                Title = createArticleDto.Title,
                Text = createArticleDto.Text,
                UserId = id,
                Rank = 0,
            };

            if (createArticleDto.ImageFile != null)
            {
                article.ImageName = await _imageService.SaveImage(createArticleDto.ImageFile);
            }

            await _articleRepository.CreateAsync(article);

            var createdArticleDto = new ArticleDto(article.Id, article.Title, article.ImageName, article.Text);
            return new CreatedResult("", createdArticleDto);
        }

        public async Task<ActionResult<ArticleDto>> UpdateArticle(int articleId, UpdateArticleDto updateArticleDto, ClaimsPrincipal user)
        {
            var id = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var currentUser = await _authService.GetUserById(id);

            if (!currentUser.CanWriteArticles)
            {
                return new ForbidResult();
            }

            var article = await _articleRepository.GetAsync(articleId);

            if (article == null)
            {
                return new NotFoundResult();
            }


            if (!user.IsInRole(BlogRoles.Admin) && id != article.UserId)
            {
                return new ForbidResult();
            }


            if (id != article.UserId)
            {
                return new ForbidResult();
            }

            article.Title = updateArticleDto.Title;
            article.Text = updateArticleDto.Text;

            await _articleRepository.UpdateAsync(article);

            var updatedArticleDto = new ArticleDto(article.Id, article.Title, article.ImageName, article.Text);
            return new OkObjectResult(updatedArticleDto);
        }

        public async Task<ActionResult> DeleteArticle(int articleId, ClaimsPrincipal user)
        {
            var id = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var currentUser = await _authService.GetUserById(id);

            if (!currentUser.CanWriteArticles)
            {
                return new ForbidResult();
            }

            var article = await _articleRepository.GetAsync(articleId);

            if (article == null)
            {
                return new NotFoundResult();
            }

            if (!user.IsInRole(BlogRoles.Admin) && id != article.UserId)
            {
                return new ForbidResult();
            }

            if (article.ImageName != null)
            {
                _imageService.DeleteImage(article.ImageName);
            }

            await _articleRepository.DeleteAsync(article);

            return new NoContentResult();
        }

        public async Task<int?> VoteArticle(int articleId, string userId, bool isUpVote)
        {
            var article = await _articleRepository.GetAsync(articleId);

            if (article == null)
            {
                return null;
            }

            var existingVote = await _userArticleVoteRepository.GetAsync(articleId, userId);

            if (existingVote != null)
            {
                if (existingVote.IsUpVote == isUpVote)
                {
                    return null;
                }

                existingVote.IsUpVote = isUpVote;
                await _userArticleVoteRepository.UpdateAsync(existingVote);
            }
            else
            {
                var newUserVote = new UserArticleVote
                {
                    UserId = userId,
                    ArticleId = articleId,
                    IsUpVote = isUpVote
                };

                await _userArticleVoteRepository.CreateAsync(newUserVote);
            }

            await UpdateArticleRank(articleId);

            return article.Rank;
        }

        public async Task UpdateArticleRank(int articleId)
        {
            var upvotes = await _userArticleVoteRepository.CountArticleVotes(articleId, true);
            var downvotes = await _userArticleVoteRepository.CountArticleVotes(articleId, false);

            var article = await _articleRepository.GetAsync(articleId);

            if (article != null)
            {
                article.Rank = upvotes - downvotes;

                await _articleRepository.UpdateAsync(article);
            }
        }
    }
}
