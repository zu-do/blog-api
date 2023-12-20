using Microsoft.AspNetCore.Mvc;
using TheBlog_API.Data.Dtos;
using TheBlog_API.Services.Interfaces;

namespace TheBlog_API.Controllers
{
    [ApiController]
    [Route("api/Articles/{articleId}/Comments")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IEnumerable<RetrievedCommentDto>> GetAll(int articleId)
        {
            var comments = await _commentService.GetAllComments(articleId);
            return comments;
        }

        [HttpGet]
        [Route("{commentId}")]
        public async Task<ActionResult<RetrievedCommentDto>> Get(int articleId, int commentId)
        {
            var result = await _commentService.GetComment(articleId, commentId);

            return result;
        }

        [HttpPost]
        public async Task<ActionResult<RetrievedCommentDto>> Create(int articleId, CreateCommentDto createCommentDto)
        {
            var result = await _commentService.CreateComment(articleId, createCommentDto, User);
            return result;
        }

        [HttpPatch]
        [Route("{commentId}")]
        public async Task<ActionResult<CommentDto>> Update(int articleId, int commentId, UpdateCommentDto createCommentDto)
        {
            var result = await _commentService.UpdateComment(articleId, commentId, createCommentDto, User);
            return result;
        }

        [HttpDelete]
        [Route("{commentId}")]
        public async Task<ActionResult> Delete(int articleId, int commentId)
        {
            var result = await _commentService.DeleteComment(articleId, commentId, User);
            return result;
        }

        [HttpPost("{commentId}/report")]
        public async Task<ActionResult<CommentDto>> Report(int articleId, int commentId)
        {
            var result = await _commentService.ReportComment(articleId, commentId, User);
            return result;
        }

        [HttpPost("{commentId}/block")]
        public async Task<ActionResult<CommentDto>> Block(int articleId, int commentId, BlockCommentDto block)
        {
            var result = await _commentService.BlockComment(articleId, commentId, block.IsBlocked, User);
            return result;
        }
    }
}
