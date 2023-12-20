namespace TheBlog_API.Data.Dtos
{
    public record CommentDto(int Id, string Content, bool? IsBlocked);
    public record RetrievedCommentDto(int Id, DateTime CreatedAt, string Content, string AuthorUserName, bool? IsBlocked, int? ReportCount);
    public record ReportedCommentDto(int Id, DateTime CreatedAt, string Content, string AuthorUserName, bool? IsBlocked, int? ReportCount, int ArticleId);
    public record CreatedCommentDto(int Id, string Content, DateTime CreatedAt, string AuthorUserName, bool? IsBlocked);
    public record UpdateCommentDto(string Content);
    public record CreateCommentDto(string Content);
    public record BlockCommentDto(bool IsBlocked);
}
