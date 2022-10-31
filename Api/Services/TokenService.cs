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
    public class TokenService : ITokenService {
        private readonly IMapper _mapper;
        private readonly DAL.DataContext _context;
        private readonly AuthConfig _config;
		private readonly IUserService userService;

        public TokenService(IMapper mapper, DataContext context, IOptions<AuthConfig> config, IUserService userService)
        {
            _mapper = mapper;
            _context = context;
            _config = config.Value;
			this.userService = userService;
        }

		public async Task<TokenModel> GetToken(string login, string password) {
			var user = await userService.GetUserByCredention(login, password);

			return GenerateTokens(user);
		}

		public async Task<TokenModel> GetTokenByRefreshToken(string refreshToken) {
			var validParams = new TokenValidationParameters {
				ValidateAudience = false,
				ValidateIssuer = false,
				ValidateIssuerSigningKey = true,
				ValidateLifetime = true,
				IssuerSigningKey = _config.SymmetricSecurityKey()
			};
			var principal = new JwtSecurityTokenHandler().ValidateToken(refreshToken, validParams, out var securityToken);

			if (securityToken is not JwtSecurityToken jwtToken
				|| !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
				StringComparison.InvariantCultureIgnoreCase)) {
				throw new SecurityTokenException("invalid token");
			}

			if (principal.Claims.FirstOrDefault(x => x.Type == "id")?.Value is String userIdString
				&& Guid.TryParse(userIdString, out var userId)) {
				var user = await userService.GetUserById(userId);
				return GenerateTokens(user);
			} else {
				throw new SecurityTokenException("invalid token");
			}
		}

		private TokenModel GenerateTokens(DAL.Entities.User user) {
			var dtNow = DateTime.Now;

			var jwt = new JwtSecurityToken(
				issuer: _config.Issuer,
				audience: _config.Audience,
				notBefore: dtNow,
				claims: new Claim[] {
			new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
			new Claim("id", user.Id.ToString()),
			},
				expires: DateTime.Now.AddMinutes(_config.LifeTime),
				signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
				);
			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			var refresh = new JwtSecurityToken(
				notBefore: dtNow,
				claims: new Claim[] {
				new Claim("id", user.Id.ToString()),
				},
				expires: DateTime.Now.AddHours(_config.LifeTime),
				signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
				);
			var encodedRefresh = new JwtSecurityTokenHandler().WriteToken(refresh);

			return new TokenModel(encodedJwt, encodedRefresh);

		}


	}
}
