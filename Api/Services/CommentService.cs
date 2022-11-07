using Api.Configs;
using Api.Models;
using AutoMapper;
using DAL;
using Microsoft.Extensions.Options;

namespace Api.Services
{
	public class CommentService : IDisposable
	{
		private readonly IMapper mapper;
		private readonly DAL.DataContext context;
		private readonly PostService postService;
		private readonly UserService userService;

		public CommentService(IMapper mapper, IOptions<AuthConfig> config, DataContext context, PostService postService, UserService userService)
		{
			this.mapper = mapper;
			this.context = context;
			this.postService = postService;
			this.userService = userService;
		}

		public async Task<Guid> AddCommentToPost(CreateCommentModel model, Guid userId)
		{
			var comment = mapper.Map<DAL.Entities.Comment>(model);

			var user = await userService.GetUserById(userId);
			comment.Author = user;
			var post = await postService.GetPostById(model.PostId, userId);
			comment.Post = post;

			var t = await context.Comments.AddAsync(comment);

			await context.SaveChangesAsync();
			return t.Entity.Id;
		}

		public async Task DeleteCommentFromPost(Guid id, Guid userId)
		{
			var post = await postService.GetPostById(id, userId);
			if (post != null)
			{
				context.Posts.Remove(post);
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