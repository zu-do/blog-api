using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheBlog_API.Data.Dtos;

namespace TheBlog_API.Services.Interfaces
{
    public interface ICommentService
    {
        Task<ActionResult<CommentDto>> BlockComment(int articleId, int commentId, bool value, ClaimsPrincipal user);
        Task<ActionResult<RetrievedCommentDto>> CreateComment(int articleId, CreateCommentDto createCommentDto, ClaimsPrincipal user);
        Task<ActionResult> DeleteComment(int articleId, int commentId, ClaimsPrincipal user);
        Task<IEnumerable<RetrievedCommentDto>> GetAllComments(int articleId);
        Task<IActionResult> GetAllReportedComments(ClaimsPrincipal user);
        Task<ActionResult<RetrievedCommentDto>> GetComment(int articleId, int commentId);
        Task<ActionResult<CommentDto>> ReportComment(int articleId, int commentId, ClaimsPrincipal user);
        Task<ActionResult<CommentDto>> UpdateComment(int articleId, int commentId, UpdateCommentDto updateCommentDto, ClaimsPrincipal user);
    }
}