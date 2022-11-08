using Api.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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

		public async Task ChangePost(Guid postId, Guid userId, string text)
		{
			var post = await GetPostById(postId, userId);
			if (post != null)
			{
				post.Text = text;
				await context.SaveChangesAsync();
			}
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

		public async Task<DAL.Entities.Post> GetPostById(Guid id, Guid? userId = null)
		{
			var post = await context.Posts.Include(c => c.Photos).FirstOrDefaultAsync(x => x.Id == id);
			if (userId != null) {
				post = await context.Posts.Include(c => c.Comments).Where(p => p.Author != null && p.Author.Id == userId).FirstOrDefaultAsync(x => x.Id == id);
			}
			if (post == null)
				throw new Exception("post is not found");
			return post;
		}

		public async Task<List<PostModel>> GetCurrentUserPosts(Guid userId)
		{
			return await context.Posts.Where(p => p.Author != null && p.Author.Id == userId).AsNoTracking().ProjectTo<PostModel>(mapper.ConfigurationProvider).ToListAsync();
		}

		public async Task<List<PostModel>> GetPosts()
		{
			return await context.Posts.Select().AsNoTracking().ProjectTo<PostModel>(mapper.ConfigurationProvider).ToListAsync();
		}

		public async Task AddPhotoToPost(Guid postId, Guid userId, MetadataModel meta, string filePath)
		{
			try
			{
				var user = await userService.GetUserById(userId);
				var post = await context.Posts.Include(x => x.Photos).FirstOrDefaultAsync(x => x.Id == postId);
				var photo = new Photo { Post = post, Author = user, MimeType = meta.MimeType, FilePath = filePath, Name = meta.Name, Size = meta.Size };
				post?.Photos?.Add(photo);
			}
			catch (NullReferenceException ex)
			{
				throw new NullReferenceException(ex.Message);
			}
			finally 
			{ 
				await context.SaveChangesAsync(); 
			}
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