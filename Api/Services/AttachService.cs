using Api.Configs;
using Api.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Api.Services
{
	public class AttachService : IDisposable
	{
		private readonly IMapper mapper;
		private readonly DAL.DataContext context;
		private readonly PostService postService;
		private readonly UserService userService;

		public AttachService(IMapper mapper, IOptions<AuthConfig> config, DataContext context, PostService postService, UserService userService)
		{
			this.mapper = mapper;
			this.context = context;
			this.postService = postService;
			this.userService = userService;
		}

		//public async Task AddPhotoToPost(Guid postId, MetadataModel meta, string filePath)
		//{
		//	var post = await context.Posts.Include(x => x.Photo).FirstOrDefaultAsync(x => x.Id == postId);
		//	if (post != null)
		//	{
		//		var photo = new Photo { Post = post, MimeType = meta.MimeType, FilePath = filePath, Name = meta.Name, Size = meta.Size };
		//		post.Photo.Add(photo);

		//		await context.SaveChangesAsync();
		//	}

		//}

		//public async Task<List<UserModel>> GetFiles()
		//{
		//	return await context.AsNoTracking().ProjectTo<UserModel>(mapper.ConfigurationProvider).ToListAsync();
		//}

		public async Task AddAvatarToUser(Guid userId, MetadataModel meta, string filePath)
		{
			var user = await context.Users.Include(x => x.Avatar).FirstOrDefaultAsync(x => x.Id == userId);
			if (user != null)
			{
				var avatar = new Avatar { Author = user, MimeType = meta.MimeType, FilePath = filePath, Name = meta.Name, Size = meta.Size };
				user.Avatar = avatar;

				await context.SaveChangesAsync();
			}
		}

		public async Task<AttachModel> GetUserAvatar(Guid userId)
		{
			var user = await userService.GetUserById(userId);
			var attach = mapper.Map<AttachModel>(user.Avatar);
			return attach;
		}

		public async Task<AttachModel> GetPictureFromPost(Guid pictureId, Guid postId)
		{
			var post = await postService.GetPostById(postId);
			var attach = mapper.Map<AttachModel>(post.Photos?.FirstOrDefault(p => p.Id == pictureId));
			
			return attach;
		}

		//public async Task<AttachModel> GetPostPhotos(Guid postId)
		//{
		//	var post = await postService.GetPostById(postId);
		//	var attach = mapper.Map<AttachModel>(post.Photos);
		//	return attach;
		//}

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