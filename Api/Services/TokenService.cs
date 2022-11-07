using Api.Configs;
using Api.Models;
using DAL;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Services
{
	public class TokenService : IDisposable
	{
		private readonly DAL.DataContext context;
		private readonly AuthConfig config;
		private readonly UserService userService;
		private readonly SessionService sessionService;

		public TokenService(IOptions<AuthConfig> config, DataContext context, UserService userService, SessionService sessionService)
		{
			this.context = context;
			this.config = config.Value;
			this.userService = userService;
			this.sessionService = sessionService;
		}

		private TokenModel GenerateTokens(DAL.Entities.UserSession session)
		{
			var dtNow = DateTime.Now;
			if (session.User == null)
				throw new Exception("magic");

			var jwt = new JwtSecurityToken(
				issuer: config.Issuer,
				audience: config.Audience,
				notBefore: dtNow,
				claims: new Claim[] {
			new Claim(ClaimsIdentity.DefaultNameClaimType, session.User.Name),
			new Claim("sessionId", session.Id.ToString()),
			new Claim("id", session.User.Id.ToString()),
			},
				expires: DateTime.Now.AddMinutes(config.LifeTime),
				signingCredentials: new SigningCredentials(config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
				);
			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			var refresh = new JwtSecurityToken(
				notBefore: dtNow,
				claims: new Claim[] {
				new Claim("refreshToken", session.RefreshToken.ToString()),
				},
				expires: DateTime.Now.AddHours(config.LifeTime),
				signingCredentials: new SigningCredentials(config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
				);
			var encodedRefresh = new JwtSecurityTokenHandler().WriteToken(refresh);

			return new TokenModel(encodedJwt, encodedRefresh);
		}

		public async Task<TokenModel> GetToken(string login, string password)
		{
			var user = await userService.GetUserByCredention(login, password);
			var session = await context.UserSessions.AddAsync(new DAL.Entities.UserSession
			{
				User = user,
				RefreshToken = Guid.NewGuid(),
				Created = DateTime.UtcNow,
				Id = Guid.NewGuid()
			});
			await context.SaveChangesAsync();
			return GenerateTokens(session.Entity);
		}

		public async Task<TokenModel> GetTokenByRefreshToken(string refreshToken)
		{
			var validParams = new TokenValidationParameters
			{
				ValidateAudience = false,
				ValidateIssuer = false,
				ValidateIssuerSigningKey = true,
				ValidateLifetime = true,
				IssuerSigningKey = config.SymmetricSecurityKey()
			};
			var principal = new JwtSecurityTokenHandler().ValidateToken(refreshToken, validParams, out var securityToken);

			if (securityToken is not JwtSecurityToken jwtToken
				|| !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
				StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("invalid token");
			}

			if (principal.Claims.FirstOrDefault(x => x.Type == "refreshToken")?.Value is String refreshIdString
				&& Guid.TryParse(refreshIdString, out var refreshId)
				)
			{
				var session = await sessionService.GetSessionByRefreshToken(refreshId);
				if (!session.IsActive)
				{
					throw new Exception("session is not active");
				}

				session.RefreshToken = Guid.NewGuid();
				await context.SaveChangesAsync();

				return GenerateTokens(session);
			}
			else
			{
				throw new SecurityTokenException("invalid token");
			}
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