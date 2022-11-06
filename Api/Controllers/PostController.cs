using Api.Models;
using Api.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Api.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class PostController : CommonController
	{

		private readonly PostService postService;

		public PostController(PostService postService)
		{
			this.postService = postService;
		}

		[HttpPost]
		[Authorize]
		public async Task CreatePost(CreatePostModel model) => await postService.CreatePost(model, GetCurrentUser());

		[Authorize]
		[HttpPost]
		public async Task DeletePost(Guid id) => await postService.DeletePost(id, GetCurrentUser());

		[HttpGet]
		public async Task<List<PostModel>> GetAllPosts() => await postService.GetPosts();

		[HttpGet]
		[Authorize]
		public async Task<List<PostModel>> GetCurrentUserPosts() => await postService.GetCurrentUserPosts(GetCurrentUser());
	}
}
