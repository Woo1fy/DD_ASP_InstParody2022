using Api.Models;

namespace Api.Services.Abstract {
	public interface IUserService {
		Task CreateUser(CreateUserModel model);

		Task<TokenModel> GetTokenByRefreshToken(string refreshToken);

		Task<TokenModel> GetToken(string login, string password);

		Task<UserModel> GetUser(Guid id);

		Task<List<UserModel>> GetUsers();
	}
}
