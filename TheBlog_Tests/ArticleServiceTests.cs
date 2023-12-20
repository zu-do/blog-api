using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Moq;
using Microsoft.IdentityModel.JsonWebTokens;
using TheBlog_API.Data.Dtos;
using TheBlog_API.Data.Interfaces;
using TheBlog_API.Data.Models;
using TheBlog_API.Services;
using TheBlog_API.Services.Interfaces;

namespace TheBlog_Tests
{
    public class ArticleServiceTests
    {
        private readonly IArticleService _articleService;
        private readonly Mock<IArticleRepository> _articleRepository;
        private readonly Mock<IUserArticleVoteRepository> _userArticleVoteRepository = new Mock<IUserArticleVoteRepository>();
        private readonly Mock<IAuthService> _authService;
        private readonly Mock<IImageService> _imageService;

        public ArticleServiceTests()
        {
            _articleRepository = MockArticleRepository(_articles);
            
            _authService = new Mock<IAuthService>();
            _authService.Setup(x => x.GetUserById("userId")).ReturnsAsync(new ApplicationUser { CanWriteArticles = true });
            _imageService = new Mock<IImageService>();
            _imageService.Setup(x => x.SaveImage(It.IsAny<IFormFile>())).ReturnsAsync("testImage.jpg");
            _articleService = new ArticleService(_articleRepository.Object, _userArticleVoteRepository.Object, _authService.Object, _imageService.Object);
        }

        public static Mock<IArticleRepository> MockArticleRepository(List<Article> articles)
        {
            var mockRepository = new Mock<IArticleRepository>();

            mockRepository.Setup(repo => repo.GetAsync(It.IsAny<int>()))
                          .ReturnsAsync((int articleId) => articles.FirstOrDefault(a => a.Id == articleId));
            mockRepository.Setup(repo => repo.GetAllAsync())
                          .ReturnsAsync(articles);
            mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<Article>()))
                          .Callback<Article>((article) => articles.Add(article))
                          .Returns(Task.CompletedTask);
            mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Article>()))
                          .Returns(Task.CompletedTask);
            mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Article>()))
                          .Callback<Article>((article) =>
                          {
                              articles.Remove(article);
                          })
                          .Returns(Task.CompletedTask);

            return mockRepository;
        }

        private List<Article> _articles = new List<Article>
        {
            new Article { Id = 1, Title = "Article 1", Rank = 1, ImageName = "image1.jpg", Text = "Text 1", User = new ApplicationUser { Id="userId", UserName = "TestName"}, Comments = new List<Comment> { new Comment { CreatedAt = DateTime.UtcNow } } },
            new Article { Id = 2, Title = "Article 2", Rank = 2, ImageName = "image2.jpg", Text = "Text 2", User = new ApplicationUser { Id="userId", UserName = "TestName"}, UserId="userId", Comments = new List<Comment>() }
        };

        [Fact]
        public async Task GetAllArticles_ShouldReturnArticleList()
        {
            var result = await _articleService.GetAllArticles("https", "example.com", "/base");

            foreach (var article in result)
            {
                Console.WriteLine($"Article ID: {article.Id}, Title: {article.Title}");
            }

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var retrievedArticleDto = result.First();
            Assert.Equal(1, retrievedArticleDto.Id);
            Assert.Equal("Article 1", retrievedArticleDto.Title);
            Assert.Equal(1, retrievedArticleDto.Rank);
            Assert.Equal("image1.jpg", retrievedArticleDto.ImageName);
            Assert.Equal("Text 1", retrievedArticleDto.Text);
            Assert.Equal("https://example.com/base/Images/image1.jpg", retrievedArticleDto.ImageSrc);
            Assert.NotNull(retrievedArticleDto.LastCommentedAt);
            Assert.Equal(DateTime.UtcNow.Date, retrievedArticleDto.LastCommentedAt?.Date);
        }

        [Fact]
        public async Task GetArticleById_ShouldReturnArticle()
        {
            var retrievedArticle = await _articleService.GetArticle(2, "https", "example.com", "/base");

            Assert.NotNull(retrievedArticle.Value);

            var retrievedArticleDto = retrievedArticle.Value;

            Assert.Equal(2, retrievedArticleDto.Id);
            Assert.Equal("Article 2", retrievedArticleDto.Title);
            Assert.Equal(2, retrievedArticleDto.Rank);
            Assert.Equal("image2.jpg", retrievedArticleDto.ImageName);
            Assert.Equal("Text 2", retrievedArticleDto.Text);
            Assert.Equal("https://example.com/base/Images/image2.jpg", retrievedArticleDto.ImageSrc);
            Assert.Null(retrievedArticleDto.LastCommentedAt);
        }

        [Fact]
        public async Task GetArticleByIdWithNonExistenceId_ShouldReturnNotFound()
        {
            var retrievedArticle = await _articleService.GetArticle(22, "https", "example.com", "/base");

            Assert.Null(retrievedArticle.Value);
            Assert.IsType<NotFoundResult>(retrievedArticle.Result);
        }

        [Fact]
        public async Task UpdateArticle_ShouldReturnOkAndObject()
        {
            var updateArticleDto = new UpdateArticleDto("Updated Title", "Updated Text");

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "userId"),
            }));

            var update = await _articleService.UpdateArticle(2, updateArticleDto, user);

            Assert.IsType<OkObjectResult>(update.Result);
            var okResult = update.Result as OkObjectResult;
            var updatedArticleDto = okResult?.Value as ArticleDto;

            Assert.NotNull(updatedArticleDto);
            Assert.Equal(2, updatedArticleDto.Id);
            Assert.Equal("Updated Title", updatedArticleDto.Title);
            Assert.Equal("image2.jpg", updatedArticleDto.ImageName);
            Assert.Equal("Updated Text", updatedArticleDto.Text);
        }

        [Fact]
        public async Task UpdateArticleForNotOwner_ShouldReturnForbidResult()
        {
            var updateArticleDto = new UpdateArticleDto("Updated Title", "Updated Text");

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "userId"),
            }));

            var update = await _articleService.UpdateArticle(1, updateArticleDto, user);

            Assert.IsType<ForbidResult>(update.Result);
        }

        [Fact]
        public async Task CreateArticle_ShouldReturnCreatedAndObject()
        {
            var mockedImageFile = new Mock<IFormFile>();
            var createArticleDto = new CreateArticleDto( "New Article", "newImage.png", "Article Text", mockedImageFile.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "userId"),
            }));

            var create = await _articleService.CreateArticle(createArticleDto, user);

            Assert.IsType<CreatedResult>(create.Result);

            var createdResult = create.Result as CreatedResult;
            Assert.NotNull(createdResult);

            var createdArticleDto = createdResult.Value as ArticleDto;
            Assert.NotNull(createdArticleDto);

            Assert.Equal("New Article", createdArticleDto.Title);
            Assert.Equal("testImage.jpg", createdArticleDto.ImageName);
            Assert.Equal("Article Text", createdArticleDto.Text);

            _articleRepository.Verify(repo => repo.CreateAsync(It.IsAny<Article>()), Times.Once);

            _imageService.Verify(imageService => imageService.SaveImage(It.IsAny<IFormFile>()), Times.Once);
        }


        [Fact]
        public async Task DeleteArticle_ShouldReturnNoContentResult()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "userId"),
            }));

            var result = await _articleService.DeleteArticle(2, user);

            Assert.Single(_articles);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteArticle_ThatNotExists_ShouldReturnNoFoundResult()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "userId"),
            }));

            var result = await _articleService.DeleteArticle(4, user);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteArticle_ByNonCreator_ShouldReturnForbidResult()
        {

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "userId"),
            }));

            var result = await _articleService.DeleteArticle(1, user);

            Assert.Equal(2, _articles.Count);
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task VoteArticle_ShouldReturnUpdatedRank()
        {
            var articleId = 1;
            var userId = "testUserId";
            var isUpVote = true;


            _userArticleVoteRepository.Setup(repo => repo.GetAsync(articleId, userId)).ReturnsAsync((UserArticleVote)null);

            UserArticleVote createdUserVote = null;
            _userArticleVoteRepository.Setup(repo => repo.CreateAsync(It.IsAny<UserArticleVote>()))
                                     .Callback<UserArticleVote>(vote => createdUserVote = vote)
                                     .Returns(Task.CompletedTask);

            UserArticleVote updatedUserVote = null;
            _userArticleVoteRepository.Setup(repo => repo.UpdateAsync(It.IsAny<UserArticleVote>()))
                                     .Callback<UserArticleVote>(vote => updatedUserVote = vote)
                                     .Returns(Task.CompletedTask);

            var updatedRank = await _articleService.VoteArticle(articleId, userId, isUpVote);

            Assert.NotNull(updatedRank);
            Assert.Equal(_articles[0].Rank, updatedRank);

            if (createdUserVote != null)
            {
                Assert.Equal(userId, createdUserVote.UserId);
                Assert.Equal(articleId, createdUserVote.ArticleId);
                Assert.Equal(isUpVote, createdUserVote.IsUpVote);
            }

            if (updatedUserVote != null)
            {
                Assert.Equal(userId, updatedUserVote.UserId);
                Assert.Equal(articleId, updatedUserVote.ArticleId);
                Assert.Equal(isUpVote, updatedUserVote.IsUpVote);
            }

            _articleRepository.Verify(repo => repo.GetAsync(articleId), Times.AtLeastOnce);

            _userArticleVoteRepository.Verify(repo => repo.GetAsync(articleId, userId), Times.Once);

            if (createdUserVote != null)
            {
                _userArticleVoteRepository.Verify(repo => repo.CreateAsync(It.IsAny<UserArticleVote>()), Times.Once);
            }
            else if (updatedUserVote != null)
            {
                _userArticleVoteRepository.Verify(repo => repo.UpdateAsync(It.IsAny<UserArticleVote>()), Times.Once);
            }

        }

        [Fact]
        public async Task VoteAlreadytVotedArticle_ReturnsNull()
        {
            var isUpVote = true;

            _userArticleVoteRepository.Setup(repo => repo.GetAsync(1, "userId")).ReturnsAsync(new UserArticleVote { IsUpVote = true });

            var updatedRank = await _articleService.VoteArticle(1, "userId", isUpVote);

            Assert.Null(updatedRank);
        }

        [Fact]
        public async Task VoteNonExistingArticle_ReturnsNull()
        {
            var isUpVote = true;

            var updatedRank = await _articleService.VoteArticle(4, "userId", isUpVote);

            Assert.Null(updatedRank);
        }
    }
}
