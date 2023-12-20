using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TheBlog_API.Data.Dtos;
using TheBlog_API.Data.Interfaces;
using TheBlog_API.Data.Models;
using TheBlog_API.Data.Repositories;
using TheBlog_API.Services;
using TheBlog_API.Services.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TheBlog_Tests
{
    public class CommentServiceTests
    {
        private readonly ICommentService _commentService;
        private readonly Mock<ICommentRepository> _commentRepository;
        private readonly Mock<IAuthService> _authService;
        private readonly Mock<IReportRepository> _reportRepository;
        private readonly Mock<IArticleRepository> _articleRepository;
        public CommentServiceTests()
        {
            _commentRepository = MockCommentRepository(_comments);
            _authService = new Mock<IAuthService>();
            _reportRepository = new Mock<IReportRepository>();
            _authService = new Mock<IAuthService>();
            _articleRepository = new Mock<IArticleRepository>();

            _authService.Setup(x => x.GetUserById("userId")).ReturnsAsync(new ApplicationUser { CanWriteArticles = true, CanWriteComments = true });

            _articleRepository.Setup(repo => repo.GetAsync(It.Is<int>(id => id == 1)))
                  .ReturnsAsync(new Article { Id = 1 });

            _commentService = new CommentService(_commentRepository.Object, _articleRepository.Object, _authService.Object, _reportRepository.Object);
        }
        public static Mock<ICommentRepository> MockCommentRepository(List<Comment> comments)
        {
            var mockRepository = new Mock<ICommentRepository>();

            mockRepository.Setup(repo => repo.GetAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync((int articleId, int commentId) => comments.FirstOrDefault(c => c.ArticleId == articleId && c.Id == commentId));

            mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<int>()))
                          .ReturnsAsync((int articleId) => comments.Where(c => c.ArticleId == articleId).ToList());

            mockRepository.Setup(repo => repo.GetAllReportedAsync())
                          .ReturnsAsync(() => comments.Where(c => c.Reports.Count > 0).ToList());

            mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<Comment>()))
                          .Callback<Comment>((comment) => comments.Add(comment))
                          .Returns(Task.CompletedTask);

            mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Comment>()))
                          .Returns(Task.CompletedTask);

            mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Comment>()))
                          .Callback<Comment>((comment) => comments.Remove(comment))
                          .Returns(Task.CompletedTask);

            return mockRepository;
        }

        private List<Comment> _comments = new List<Comment>
        {
            new Comment { ArticleId = 1, Id = 1, Content = "Comment 1", UserId = "userId", User = new ApplicationUser(), Reports = new List<Report>() },
            new Comment { ArticleId = 1, Id = 2, Content = "Comment 2", UserId = "userId", User = new ApplicationUser(), Reports = new List<Report>() { new Report() } },
        };

        [Fact]
        public async Task GetAllComments_ShouldReturnComments()
        {
            var articleId = 1;

            var result = await _commentService.GetAllComments(articleId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllReportedComments_ShouldReturnComments()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "userId"),
                new Claim(ClaimTypes.Role, "Admin"),
            }));

            var result = await _commentService.GetAllReportedComments(user);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var commentDtosEnumerable = Assert.IsAssignableFrom<IEnumerable<ReportedCommentDto>>(okResult.Value);

            var commentDtos = commentDtosEnumerable.ToList();
            Assert.NotNull(commentDtos);
            Assert.Single(commentDtos);
        }

        [Fact]
        public async Task GetComment_ExistingComment_ShouldReturnComment()
        {
            var articleId = 1;
            var commentId = 1;

            var result = await _commentService.GetComment(articleId, commentId);

            var actionResult = Assert.IsType<ActionResult<RetrievedCommentDto>>(result);
            Assert.NotNull(actionResult.Value);

            var retrievedCommentDto = Assert.IsType<RetrievedCommentDto>(actionResult.Value);
            Assert.Equal(commentId, retrievedCommentDto.Id);
        }

        [Fact]
        public async Task GetComment_NonExistingComment_ShouldReturnNotFoundResult()
        {
            var articleId = 1;
            var commentId = 3;

            var result = await _commentService.GetComment(articleId, commentId);

            Assert.Null(result.Value);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateComment_ShouldReturnOkAndObject()
        {
            var updateCommentDto = new UpdateCommentDto("Updated Content");

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "userId"),
            }));

            var update = await _commentService.UpdateComment(1, 1, updateCommentDto, user);

            Assert.IsType<OkObjectResult>(update.Result);
            var okResult = update.Result as OkObjectResult;
            var updatedCommentDto = okResult?.Value as CommentDto;

            Assert.NotNull(updatedCommentDto);
            Assert.Equal("Updated Content", updatedCommentDto.Content);
        }

        [Fact]
        public async Task UpdateComment_WithNonExistingComment_ShouldReturNotFound()
        {
            var updateCommentDto = new UpdateCommentDto("Updated Content");

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "userId"),
            }));

            var update = await _commentService.UpdateComment(1, 4, updateCommentDto, user);

            Assert.IsType<NotFoundResult>(update.Result);
            var okResult = update.Result as OkObjectResult;
            var updatedCommentDto = okResult?.Value as CommentDto;

            Assert.Null(updatedCommentDto);
        }

        [Fact]
        public async Task CreateComment_ShouldReturnOkAndObject()
        {
            var updateCommentDto = new CreateCommentDto("New Content");

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "userId"),
            }));

            var create = await _commentService.CreateComment(1, updateCommentDto, user);

            Assert.IsType<CreatedResult>(create.Result);
            var okResult = create.Result as CreatedResult;
            var createdCommentDto = okResult?.Value as CreatedCommentDto;

            Assert.NotNull(createdCommentDto);
            Assert.Equal("New Content", createdCommentDto.Content);
        }

        [Fact]
        public async Task CreateComment_ForNonExistingArticle_ShouldReturnNotFound()
        {
            var updateCommentDto = new CreateCommentDto("New Content");

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "userId"),
            }));

            var create = await _commentService.CreateComment(5, updateCommentDto, user);

            Assert.IsType<NotFoundResult>(create.Result);
            var okResult = create.Result as CreatedResult;
            var createdCommentDto = okResult?.Value as CreatedCommentDto;

            Assert.Null(createdCommentDto);
        }

        [Fact]
        public async Task ReportComment_ValidData_ShouldReturnOkResult()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "userId"),
            }));


            var reportRepositoryMock = new Mock<IReportRepository>();
            reportRepositoryMock.Setup(repo => repo.GetAsync(1, "userId")).ReturnsAsync((Report)null);

            var report = await _commentService.ReportComment(1, 1, user);

            Assert.IsType<OkObjectResult>(report.Result);
            Assert.Equal("Reported", (report.Result as OkObjectResult)?.Value);
        }

        [Fact]
        public async Task BlockComment_NotReportedComment_ShouldReturnBadRequest()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "userId"),
                new Claim(ClaimTypes.Role, "Admin"),
            }));

            var block = await _commentService.BlockComment(1, 1, true, user);

            Assert.IsType<BadRequestResult>(block.Result);
        }

        [Fact]
        public async Task BlockComment_BySimpleUser_ShouldReturnForbid()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "userId"),
            }));

            var block = await _commentService.BlockComment(1, 1, true, user);

            Assert.IsType<ForbidResult>(block.Result);
        }

        [Fact]
        public async Task BlockComment_ReportedComment_ShouldReturnOk()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "userId"),
                new Claim(ClaimTypes.Role, "Admin"),
            }));

            var block = await _commentService.BlockComment(1, 2, true, user);

            Assert.IsType<OkObjectResult>(block.Result);
            var okResult = block.Result as OkObjectResult;
            var reportedComment = okResult?.Value as CommentDto;

            Assert.NotNull(reportedComment);    
            Assert.True(reportedComment.IsBlocked);
        }

        [Fact]
        public async Task DeleteComment__ShouldReturnNoContent()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "userId"),
            })); ;

            var result = await _commentService.DeleteComment(1, 1, user);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteNonExistingComment_ShouldReturnNotFound()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "userId"),
            })); ;

            var result = await _commentService.DeleteComment(1, 4, user);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
