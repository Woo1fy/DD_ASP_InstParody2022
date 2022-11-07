using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class AuthController : CommonController
	{
		private readonly TokenService tokenService;

		public AuthController(TokenService tokenService)
		{
			this.tokenService = tokenService;
		}

		[HttpPost]
		public async Task<TokenModel> Token(TokenRequestModel model)
			=> await tokenService.GetToken(model.Login, model.Pass);

		[HttpPost]
		public async Task<TokenModel> RefreshToken(RefreshTokenRequestModel model)
			=> await tokenService.GetTokenByRefreshToken(model.RefreshToken);
	}
}