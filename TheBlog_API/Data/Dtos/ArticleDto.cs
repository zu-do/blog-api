using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBlog_API.Data.Dtos
{
    public record ArticleDto(int Id, string Title, string? ImageName, string? Text);
    public record RetrievedArticleDto(int Id, string Title, string? AuthorId, string? AuthorUsername, 
        int? Rank, string? ImageName, string? Text, string? ImageSrc, DateTime? LastCommentedAt);
    public record UpdateArticleDto(string Title, string? Text);
    public record CreateArticleDto
    (
        [Required(ErrorMessage = "Title is required")]string Title, 
        string? ImageName, 
        string? Text,
        IFormFile? ImageFile
    );
}
