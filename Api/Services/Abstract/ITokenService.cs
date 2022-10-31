using Api.Models;

namespace Api.Services.Abstract {
	public interface ITokenService {
		Task<TokenModel> GetTokenByRefreshToken(string refreshToken);

		Task<TokenModel> GetToken(string login, string password);
	}
}
