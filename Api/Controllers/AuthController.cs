using Api.Models;
using Api.Services;
using Api.Services.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService tokenService;

        public AuthController(ITokenService tokenService)
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
