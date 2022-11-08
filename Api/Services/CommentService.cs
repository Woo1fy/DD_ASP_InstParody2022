using Api.Configs;
using Api.Models;
using AutoMapper;
using DAL;
using Microsoft.EntityFrameworkCore;
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
			var post = await postService.GetPostById(model.PostId);
			comment.Post = post;

			var t = await context.Comments.AddAsync(comment);

			await context.SaveChangesAsync();
			return t.Entity.Id;
		}

		// Не знаю, правильно находить по идентификатору поста коммент, или же только по своему идентификатору.
		// Я думал так. Раз пользователь хочет изменить коммент, то он сначала заходит в объект за 
		// которым он закреплён, а значит мне уже проще искать через этот объект определенные комментарии,
		// чем сёрчить по всей таблице комментов нужные мне.
		public async Task ChangeComment(Guid postId, Guid commentId, String text, Guid userId)
		{
			var post = await postService.GetPostById(postId, userId);
			var comment = post.Comments?.FirstOrDefault(c => c.Id == commentId);
			if (comment == null)
				throw new Exception("comment is not found");

			comment.Text = text;
			await context.SaveChangesAsync();
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