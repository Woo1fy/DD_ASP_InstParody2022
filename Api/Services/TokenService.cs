using Api.Configs;
using Api.Models;
using Api.Services.Abstract;
using AutoMapper;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Services
{
	public class TokenService : ITokenService
	{
		private readonly IMapper _mapper;
		private readonly DAL.DataContext _context;
		private readonly AuthConfig _config;
		private readonly IUserService userService;
		private readonly ILogger<ITokenService> logger;

		public TokenService(IMapper mapper, DataContext context, IOptions<AuthConfig> config, IUserService userService, ILogger<ITokenService> logger)
		{
			_mapper = mapper;
			_context = context;
			_config = config.Value;
			this.userService = userService;
			this.logger = logger;
		}

		public async Task<TokenModel> GetToken(string login, string password)
		{
			var user = await userService.GetUserByCredention(login, password);
			var session = await _context.UserSessions.AddAsync(new DAL.Entities.UserSession
			{
				User = user,
				RefreshToken = Guid.NewGuid(),
				Created = DateTime.UtcNow,
				Id = Guid.NewGuid()
			});
			await _context.SaveChangesAsync();
			return GenerateTokens(session.Entity);
		}


		public async Task<UserSession> GetSessionById(Guid id)
		{
			var session = await _context.UserSessions.FirstOrDefaultAsync(x => x.Id == id);
			if (session == null)
			{
				throw new Exception("session is not found");
			}
			return session;
		}

		private async Task<UserSession> GetSessionByRefreshToken(Guid id)
		{
			var session = await _context.UserSessions.Include(x => x.User).FirstOrDefaultAsync(x => x.RefreshToken == id);
			if (session == null)
			{
				throw new Exception("session is not found");
			}
			return session;
		}

		public async Task<TokenModel> GetTokenByRefreshToken(string refreshToken)
		{
			var validParams = new TokenValidationParameters
			{
				ValidateAudience = false,
				ValidateIssuer = false,
				ValidateIssuerSigningKey = true,
				ValidateLifetime = true,
				IssuerSigningKey = _config.SymmetricSecurityKey()
			};
			var principal = new JwtSecurityTokenHandler().ValidateToken(refreshToken, validParams, out var securityToken);

			if (securityToken is not JwtSecurityToken jwtToken
				|| !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
				StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("invalid token");
			}

			if (principal.Claims.FirstOrDefault(x => x.Type == "refreshTokee=n")?.Value is String refreshIdString
				&& Guid.TryParse(refreshIdString, out var refreshId)
				)
			{
				var session = await GetSessionByRefreshToken(refreshId);
				if (!session.IsActive)
				{
					throw new Exception("session is not active");
				}
				

				session.RefreshToken = Guid.NewGuid();
				await _context.SaveChangesAsync();
				return GenerateTokens(session);
			}
			else
			{
				throw new SecurityTokenException("invalid token");
			}
		}

		private TokenModel GenerateTokens(DAL.Entities.UserSession session)
		{
			var dtNow = DateTime.Now;
			if (session.User == null)
			{
				throw new Exception("magic");
			}
			var jwt = new JwtSecurityToken(
				issuer: _config.Issuer,
				audience: _config.Audience,
				notBefore: dtNow,
				claims: new Claim[] {
			new Claim(ClaimsIdentity.DefaultNameClaimType, session.User.Name),
			new Claim("sessionId", session.Id.ToString()),
			new Claim("id", session.User.Id.ToString()),
			},
				expires: DateTime.Now.AddMinutes(_config.LifeTime),
				signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
				);
			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			var refresh = new JwtSecurityToken(
				notBefore: dtNow,
				claims: new Claim[] {
				new Claim("refreshToken", session.RefreshToken.ToString()), 
				},
				expires: DateTime.Now.AddHours(_config.LifeTime),
				signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
				);
			var encodedRefresh = new JwtSecurityTokenHandler().WriteToken(refresh);

			return new TokenModel(encodedJwt, encodedRefresh);

		}


	}
}
