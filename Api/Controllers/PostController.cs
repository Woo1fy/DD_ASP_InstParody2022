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
	public class PostController : CommonController
	{
		private readonly PostService postService;
		private readonly CommentService commentService;

		public PostController(PostService postService, CommentService commentService)
		{
			this.postService = postService;
			this.commentService = commentService;
		}

		[HttpPost]
		[Authorize]
		public async Task CreatePost(CreatePostModel model)
			=> await postService.CreatePost(model, GetCurrentUser());

		[Authorize]
		[HttpPost]
		public async Task ChangePost(Guid postId, String text)
		=> await postService.ChangePost(postId, GetCurrentUser(), text);

		[Authorize]
		[HttpPost]
		public async Task DeletePost(Guid id)
			=> await postService.DeletePost(id, GetCurrentUser());

		[HttpGet]
		public async Task<List<PostModel>> GetAllPosts()
		{
			var postModels = await postService.GetPosts();
			
		}
			

		[HttpGet]
		[Authorize]
		public async Task<List<PostModel>> GetCurrentUserPosts()
			=> await postService.GetCurrentUserPosts(GetCurrentUser());

	}
}