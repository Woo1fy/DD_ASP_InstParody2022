using Api.Models;
using Api.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Api.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class CommentController : CommonController
	{
		private readonly PostService postService;
		private readonly CommentService commentService;

		public CommentController(PostService postService, CommentService commentService)
		{
			this.postService = postService;
			this.commentService = commentService;
		}

		[HttpPost]
		[Authorize]
		public async Task AddCommentToPost(CreateCommentModel model)
			=> await commentService.AddCommentToPost(model, GetCurrentUser());

		[HttpPost]
		[Authorize]
		public async Task ChangeComment(Guid postId, Guid commentId, String text)
			=> await commentService.ChangeComment(postId, commentId, text, GetCurrentUser());

		[Authorize]
		[HttpPost]
		public async Task DeleteCommentFromPost(Guid id)
			=> await commentService.DeleteCommentFromPost(id, GetCurrentUser());
	}
}