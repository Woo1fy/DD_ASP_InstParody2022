using Api.Configs;
using Api.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Services
{
    public class PostService
    {
        private readonly IMapper _mapper;
        private readonly DAL.DataContext _context;
        private readonly AuthConfig _config;
		private readonly UserService userService;

        public PostService(IMapper mapper, IOptions<AuthConfig> config, DataContext context, UserService userService)
        {
            _mapper = mapper;
            _context = context;
            _config = config.Value;
			this.userService = userService;
        }

		//public async Task AddPhotoToPost(Guid postId, MetadataModel metaModel, string filePath)
		//{

		//}

		public async Task<Guid> CreatePost(CreatePostModel model, Guid userId)
		{
			var post = _mapper.Map<DAL.Entities.Post>(model);

			var user = await userService.GetUserById(userId);
			post.Author = user;

			var t = await _context.Posts.AddAsync(post);

			await _context.SaveChangesAsync();
			return t.Entity.Id;
		}

		public async Task DeletePost(Guid id, Guid userId)
		{
			var post = await GetPostById(id, userId);
			if (post != null)
			{
				_context.Posts.Remove(post);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<bool> CheckPostExist(Guid id)
		{
			return await _context.Posts.AnyAsync(x => x.Id == id);
		}

		private async Task<DAL.Entities.Post> GetPostById(Guid id, Guid userId)
		{
			var post = await _context.Posts.Where(p => p.Author.Id == userId).FirstOrDefaultAsync(x => x.Id == id);
			if (post == null)
				throw new Exception("post is not found");
			return post;
		}

		//public async Task<PostModel> GetUserPosts(Guid id)
		//{
		//	var user = await userService.GetUser(id);
		//	var post = _mapper.Map<PostModel>(user);
		//	return post;
		//}

		public async Task<List<PostModel>> GetCurrentUserPosts(Guid userId)
		{
			var user = await userService.GetUserById(userId);
			return await _context.Posts.Where(p => p.Author.Id == userId).AsNoTracking().ProjectTo<PostModel>(_mapper.ConfigurationProvider).ToListAsync();
		}

		public async Task<List<PostModel>> GetPosts()
		{
			return await _context.Posts.AsNoTracking().ProjectTo<PostModel>(_mapper.ConfigurationProvider).ToListAsync();
		}

		//public async Task<UserModel> GetUserPost(Guid id)
		//{


		//}
	}
}
