using Api.Models;
using Common;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Abstract {
	public interface IUserService {
		Task CreateUser(CreateUserModel model);

		Task<UserModel> GetUser(Guid id);

		Task<List<UserModel>> GetUsers();

		Task<DAL.Entities.User> GetUserByCredention(string login, string pass);

		Task<DAL.Entities.User> GetUserById(Guid id);
	}
}
