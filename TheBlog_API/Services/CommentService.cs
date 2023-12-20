using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using TheBlog_API.Auth;
using TheBlog_API.Data.Dtos;
using TheBlog_API.Data.Interfaces;
using TheBlog_API.Data.Models;
using TheBlog_API.Data.Repositories;
using TheBlog_API.Services.Interfaces;

namespace TheBlog_API.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IAuthService _authService;

        public CommentService(ICommentRepository commentRepository, IArticleRepository articleRepository, IAuthService authService, IReportRepository reportRepository)
        {
            _commentRepository = commentRepository;
            _articleRepository = articleRepository;
            _authService = authService; 
            _reportRepository = reportRepository;
        }

        public async Task<IEnumerable<RetrievedCommentDto>> GetAllComments(int articleId)
        {
            var comments = await _commentRepository.GetAllAsync(articleId);

            return comments.Select(c => new RetrievedCommentDto(
                c.Id,
                c.CreatedAt,
                c.Content,
                c.User.UserName,
                c.IsBlocked,
                c.Reports.Count
            ));
        }

        public async Task<ActionResult<RetrievedCommentDto>> GetComment(int articleId, int commentId)
        {
            var comment = await _commentRepository.GetAsync(commentId, articleId);

            if (comment == null)
            {
                return new NotFoundResult();
            }

            var retrievedComment = new RetrievedCommentDto(
                comment.Id,
                comment.CreatedAt,
                comment.Content,
                comment.User.UserName,
                comment.IsBlocked,
                comment.Reports.Count
            );

            return new ActionResult<RetrievedCommentDto>(retrievedComment);
        }

        public async Task<ActionResult<RetrievedCommentDto>> CreateComment(int articleId, CreateCommentDto createCommentDto, ClaimsPrincipal user)
        {
            var id = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var currentUser = await _authService.GetUserById(id);

            if (!currentUser.CanWriteComments)
            {
                return new ForbidResult();
            }

            var article = await _articleRepository.GetAsync(articleId);

            if (article == null)
            {
                return new NotFoundResult();
            }

            var comment = new Comment()
            {
                Content = createCommentDto.Content,
                UserId = id,
                CreatedAt = DateTime.UtcNow,
                IsBlocked = false,
            };

            comment.ArticleId = articleId;

            await _commentRepository.CreateAsync(comment);

            var createdCommentDto = new CreatedCommentDto(comment.Id, comment.Content, comment.CreatedAt, currentUser.UserName, comment.IsBlocked);
            return new CreatedResult("", createdCommentDto);
        }

        public async Task<ActionResult<CommentDto>> UpdateComment(int articleId, int commentId, UpdateCommentDto updateCommentDto, ClaimsPrincipal user)
        {
            var id = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var currentUser = await _authService.GetUserById(id);

            if (!currentUser.CanWriteComments)
            {
                return new ForbidResult();
            }

            var article = await _articleRepository.GetAsync(articleId);

            if (article == null)
            {
                return new NotFoundResult();
            }

            var comment = await _commentRepository.GetAsync(articleId, commentId);

            if (comment == null)
            {
                return new NotFoundResult();
            }

            comment.Content = updateCommentDto.Content;

            await _commentRepository.UpdateAsync(comment);

            var updatedCommentDto = new CommentDto(comment.Id, comment.Content, comment.IsBlocked);
            return new OkObjectResult(updatedCommentDto);
        }

        public async Task<ActionResult> DeleteComment(int articleId, int commentId, ClaimsPrincipal user)
        {
            var id = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var currentUser = await _authService.GetUserById(id);

            if (!currentUser.CanWriteArticles)
            {
                return new ForbidResult();
            }

            var comment = await _commentRepository.GetAsync(articleId, commentId);

            if (comment == null)
            {
                return new NotFoundResult();
            }

            await _commentRepository.DeleteAsync(comment);

            return new NoContentResult();
        }

        public async Task<ActionResult<CommentDto>> ReportComment(int articleId, int commentId, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var article = await _articleRepository.GetAsync(articleId);

            if (article == null)
            {
                return new NotFoundResult();
            }

            var comment = await _commentRepository.GetAsync(articleId, commentId);

            if (comment == null)
            {
                return new NotFoundResult();
            }

            var existingReport = await _reportRepository.GetAsync(commentId, userId);

            if (existingReport != null)
            {
                return new BadRequestResult();
            }
            else
            {
                var newUserReport = new Report
                {
                    ApplicationUserId = userId,
                    CommentId = commentId,
                };

                await _reportRepository.CreateAsync(newUserReport);
            }

            return new OkObjectResult("Reported");
        }

        public async Task<ActionResult<CommentDto>> BlockComment(int articleId, int commentId, bool value, ClaimsPrincipal user)
        {
            if (!user.IsInRole(BlogRoles.Admin))
            {
                return new ForbidResult();
            }

            var article = await _articleRepository.GetAsync(articleId);

            if (article == null)
            {
                return new NotFoundResult();
            }

            var comment = await _commentRepository.GetAsync(articleId, commentId);

            if (comment == null)
            {
                return new NotFoundResult();
            }

            if(comment.Reports.Count == 0)
            {
                return new BadRequestResult();
            }

            comment.IsBlocked = value;

            await _commentRepository.UpdateAsync(comment);

            var updatedCommentDto = new CommentDto(comment.Id, comment.Content, comment.IsBlocked);
            return new OkObjectResult(updatedCommentDto);
        }

        public async Task<IActionResult> GetAllReportedComments(ClaimsPrincipal user)
        {
            if (!user.IsInRole(BlogRoles.Admin))
            {
                return new ForbidResult();
            }

            var comments = await _commentRepository.GetAllReportedAsync();

            var commentDtos = comments.Select(c => new ReportedCommentDto(
                c.Id,
                c.CreatedAt,
                c.Content,
                c.User.UserName,
                c.IsBlocked,
                c.Reports.Count,
                c.ArticleId
            ));

            return new OkObjectResult(commentDtos);
        }
    }
}
