using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TheBlog_API.Controllers;
using TheBlog_API.Data.Dtos;
using TheBlog_API.Services.Interfaces;
using Xunit;

namespace TheBlog_Tests;
public class ArticleControllerTests
{
    private readonly Mock<IArticleService> _articleService;
    private readonly ArticleController _articleController;

    public ArticleControllerTests()
    {
        _articleService = new Mock<IArticleService>();
        var mockHttpContext = new DefaultHttpContext();
        mockHttpContext.Request.Scheme = "http";
        mockHttpContext.Request.Host = new HostString("localhost");
        mockHttpContext.Request.PathBase = new PathString("/base");

        
        _articleController = new ArticleController(_articleService.Object);
        _articleController.ControllerContext = new ControllerContext
        {
            HttpContext = mockHttpContext
        };
    }

    [Fact]
    public async Task GetAll_ShouldReturnArticles()
    {
        _articleService.Setup(service => service.GetAllArticles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                       .ReturnsAsync(new List<RetrievedArticleDto>());

        var result = await _articleController.GetAll();

        var articles = Assert.IsAssignableFrom<IEnumerable<RetrievedArticleDto>>(result);
        Assert.NotNull(articles);
    }

    [Fact]
    public async Task Get_ShouldReturnArticle()
    {
        _articleService.Setup(service => service.GetArticle(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                       .ReturnsAsync(new RetrievedArticleDto(1, "testTitle", "testAuthorId", "testAuthorName", 2, "testImage", "testText", "test.png", DateTime.Now));

        var result = await _articleController.Get(1);

        Assert.IsType<ActionResult<RetrievedArticleDto>>(result);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedResult()
    {
        _articleService.Setup(service => service.CreateArticle(It.IsAny<CreateArticleDto>(), It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync(new CreatedResult("", new ArticleDto(1, "testTitle", "testImage", "testText")));
        var mockedImageFile = new Mock<IFormFile>();
        var result = await _articleController.Create(new CreateArticleDto("New Article", "newImage.png", "Article Text", mockedImageFile.Object));

        Assert.IsType<CreatedResult>(result.Result);
    }

    [Fact]
    public async Task Update_ShouldReturnOkResult()
    {
        _articleService.Setup(service => service.UpdateArticle(It.IsAny<int>(), It.IsAny<UpdateArticleDto>(), It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync(new OkObjectResult(new ArticleDto(1, "testTitle", "testImage", "testText")));

        var result = await _articleController.Update(1, new UpdateArticleDto("Updated Title", "Updated Text"));

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContentResult()
    {
        _articleService.Setup(service => service.DeleteArticle(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync(new NoContentResult());

        var result = await _articleController.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpvoteArticle_ShouldReturnOkResult()
    {
        _articleService.Setup(service => service.VoteArticle(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                       .ReturnsAsync(1);

        var result = await _articleController.UpvoteArticle(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task DownvoteArticle_ShouldReturnOkResult()
    {
        _articleService.Setup(service => service.VoteArticle(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                       .ReturnsAsync(1);

        var result = await _articleController.DownvoteArticle(1);

        Assert.IsType<OkObjectResult>(result);
    }
}

