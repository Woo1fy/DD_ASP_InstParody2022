using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
		public async Task DeletePost(Guid id)
			=> await postService.DeletePost(id, GetCurrentUser());

		[HttpGet]
		public async Task<List<PostModel>> GetAllPosts()
			=> await postService.GetPosts();

		[HttpGet]
		[Authorize]
		public async Task<List<PostModel>> GetCurrentUserPosts()
			=> await postService.GetCurrentUserPosts(GetCurrentUser());

		[HttpPost]
		[Authorize]
		public async Task AddCommentToPost(CreateCommentModel model)
			=> await commentService.AddCommentToPost(model, GetCurrentUser());

		[Authorize]
		[HttpPost]
		public async Task DeleteCommentFromPost(Guid id)
			=> await commentService.DeleteCommentFromPost(id, GetCurrentUser());

		[HttpPost]
		[Authorize]
		public async Task AddPhotoToPost(Guid postId, MetadataModel model)
		{
			var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), model.TempId.ToString()));
			if (!tempFi.Exists)
				throw new FileNotFoundException("file not found");
			else
			{
				var path = Path.Combine(Directory.GetCurrentDirectory(), "attaches", model.TempId.ToString());
				var destFi = new FileInfo(path);
				if (destFi.Directory != null && !destFi.Directory.Exists)
					destFi.Directory.Create();

				System.IO.File.Copy(tempFi.FullName, path, true);

				//await AttachService.AddPhotoToPost(postId, model, path);
			}
		}
	}
}