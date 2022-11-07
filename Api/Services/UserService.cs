using Api.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
	public class UserService : IDisposable
	{
		private readonly IMapper mapper;
		private readonly DAL.DataContext context;

		public UserService(IMapper mapper, DataContext context)
		{
			this.mapper = mapper;
			this.context = context;
		}

		public async Task<bool> CheckUserExist(string email)
		{
			return await context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
		}

		public async Task Delete(Guid id)
		{
			var dbUser = await GetUserById(id);
			if (dbUser != null)
			{
				context.Users.Remove(dbUser);
				await context.SaveChangesAsync();
			}
		}

		public async Task<Guid> CreateUser(CreateUserModel model)
		{
			var dbUser = mapper.Map<DAL.Entities.User>(model);
			var t = await context.Users.AddAsync(dbUser);
			await context.SaveChangesAsync();
			return t.Entity.Id;
		}

		public async Task<List<UserModel>> GetUsers()
		{
			return await context.Users.AsNoTracking().ProjectTo<UserModel>(mapper.ConfigurationProvider).ToListAsync();
		}

		public async Task<DAL.Entities.User> GetUserById(Guid id)
		{
			var user = await context.Users.Include(x => x.Avatar).FirstOrDefaultAsync(x => x.Id == id);
			if (user == null)
				throw new Exception("user not found");
			return user;
		}

		public async Task<UserModel> GetUser(Guid id)
		{
			var user = await GetUserById(id);

			return mapper.Map<UserModel>(user);
		}

		public async Task<DAL.Entities.User> GetUserByCredention(string login, string pass)
		{
			var user = await context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == login.ToLower());
			if (user == null)
				throw new Exception("user not found");

			if (!HashHelper.Verify(pass, user.PasswordHash))
				throw new Exception("password is incorrect");

			return user;
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