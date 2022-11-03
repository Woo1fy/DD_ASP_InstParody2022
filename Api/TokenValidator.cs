using Api.Services;
using DAL;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api
{
	public class TokenValidatorMiddleware
	{
		private readonly RequestDelegate next;
		public TokenValidatorMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext context, TokenService tokenService)
		{
			var isOk = true;
			var sessionIdString = context.User.Claims.FirstOrDefault(x => x.Type == "sessionId")?.Value;
			if (Guid.TryParse(sessionIdString, out var sessionId))
			{
				var session = await tokenService.GetSessionById(sessionId);
				if (!session.IsActive)
					isOk = false;
				context.Response.Clear();
				context.Response.StatusCode = 401;
			}

			if (isOk)
			{
				await next(context);
			}
		}
	}
			//var principal = new JwtSecurityTokenHandler().ValidateToken()

	public static class TokenValidatorMiddlewareExtenisions
	{
		public static IApplicationBuilder UseTokenValidator(
			this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<TokenValidatorMiddleware>();
		}
	}
}
