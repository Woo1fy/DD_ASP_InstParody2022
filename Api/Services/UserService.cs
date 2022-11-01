using Api.Configs;
using Api.Models;
using Api.Services.Abstract;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly DAL.DataContext _context;
        private readonly AuthConfig _config;
		private readonly ILogger<IUserService> logger;

        public UserService(IMapper mapper, DataContext context, IOptions<AuthConfig> config, ILogger<IUserService> logger)
        {
            _mapper = mapper;
            _context = context;
            _config = config.Value;
			this.logger = logger;
        }

		public async Task CreateUser(CreateUserModel model) {
			var dbUser = _mapper.Map<DAL.Entities.User>(model);
			await _context.Users.AddAsync(dbUser);
			await _context.SaveChangesAsync();
		}

		public async Task<List<UserModel>> GetUsers()
        {
            return await _context.Users.AsNoTracking().ProjectTo<UserModel>(_mapper.ConfigurationProvider).ToListAsync();
        }

       
        public async Task<UserModel> GetUser(Guid id)
        {
            var user = await GetUserById(id);

            return _mapper.Map<UserModel>(user);
        }

		

   

		#region HelpMethods

		public async Task<DAL.Entities.User> GetUserByCredention(string login, string pass) {
			var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == login.ToLower());
			if (user == null)
				throw new Exception("user not found");

			if (!HashHelper.Verify(pass, user.PasswordHash))
				throw new Exception("password is incorrect");

			return user;
		}

		public async Task<DAL.Entities.User> GetUserById(Guid id) {
			var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
			if (user == null)
				throw new Exception("user not found");
			return user;
		}

		#endregion

	}
}
