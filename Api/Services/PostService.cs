using Api.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
	public class PostService : IDisposable
	{
		private readonly IMapper mapper;
		private readonly DAL.DataContext context;
		private readonly UserService userService;

		public PostService(IMapper mapper, DataContext context, UserService userService)
		{
			this.mapper = mapper;
			this.context = context;
			this.userService = userService;
		}

		public async Task<Guid> CreatePost(CreatePostModel model, Guid userId)
		{
			var post = mapper.Map<DAL.Entities.Post>(model);

			var user = await userService.GetUserById(userId);
			post.Author = user;

			var t = await context.Posts.AddAsync(post);

			await context.SaveChangesAsync();
			return t.Entity.Id;
		}

		public async Task DeletePost(Guid id, Guid userId)
		{
			var post = await GetPostById(id, userId);
			if (post != null)
			{
				context.Posts.Remove(post);
				await context.SaveChangesAsync();
			}
		}

		public async Task<bool> CheckPostExist(Guid id)
		{
			return await context.Posts.AnyAsync(x => x.Id == id);
		}

		public async Task<DAL.Entities.Post> GetPostById(Guid id, Guid userId)
		{
			var post = await context.Posts.Where(p => p.Author != null && p.Author.Id == userId).FirstOrDefaultAsync(x => x.Id == id);
			if (post == null)
				throw new Exception("post is not found");
			return post;
		}

		public async Task<DAL.Entities.Post> GetPostById(Guid id)
		{
			var post = await context.Posts.Include(x => x.Comment).FirstOrDefaultAsync(x => x.Id == id);
			if (post == null)
				throw new Exception("post not found");
			return post;
		}

		public async Task<List<PostModel>> GetCurrentUserPosts(Guid userId)
		{
			return await context.Posts.Where(p => p.Author != null && p.Author.Id == userId).AsNoTracking().ProjectTo<PostModel>(mapper.ConfigurationProvider).ToListAsync();
		}

		public async Task<List<PostModel>> GetPosts()
		{
			return await context.Posts.AsNoTracking().ProjectTo<PostModel>(mapper.ConfigurationProvider).ToListAsync();
		}

		#region IDisposable Methods

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			context.Dispose();
		}

		#endregion IDisposable Methods
	}
}