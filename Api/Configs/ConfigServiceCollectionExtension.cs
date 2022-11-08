using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Api.Configs
{
	public static class ConfigServiceCollectionExtensions
	{
		public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration config)
		{
			// Add services to the container.
			var authSection = config.GetSection(AuthConfig.Position);
			var authConfig = authSection.Get<AuthConfig>();

			services.Configure<AuthConfig>(authSection);

			services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen(c =>
			{
				c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
				{
					Description = "Введите токен пользователя",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey,
					Scheme = JwtBearerDefaults.AuthenticationScheme,
				});

				c.AddSecurityRequirement(new OpenApiSecurityRequirement()
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = JwtBearerDefaults.AuthenticationScheme,
						},
						Scheme = "oauth2",
						Name = JwtBearerDefaults.AuthenticationScheme,
						In = ParameterLocation.Header,
					},
					new List<string>()
				}
			});
			});

			services.AddDbContext<DAL.DataContext>(options =>
			{
				options.UseNpgsql(config.GetConnectionString("PostgreSql"), sql => { });
			}, contextLifetime: ServiceLifetime.Scoped);

			services.AddAutoMapper(typeof(MapperProfile).Assembly);

			services.AddAuthentication(o =>
			{
				o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(o =>
			{
				o.RequireHttpsMetadata = false;
				o.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = authConfig.Issuer,
					ValidateAudience = true,
					ValidAudience = authConfig.Audience,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = authConfig.SymmetricSecurityKey(),
					ClockSkew = TimeSpan.Zero,
				};
			});

			services.AddAuthorization(o =>
			{
				o.AddPolicy("ValidAccessToken", p =>
				{
					p.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
					p.RequireAuthenticatedUser();
				});
			});

			return services;
		}

		public static IServiceCollection AddServicesGroup(this IServiceCollection services)
		{
			services.AddTransient<AttachService>();
			services.AddScoped<CommentService>();
			services.AddScoped<PostService>();
			services.AddScoped<SessionService>();
			services.AddScoped<TokenService>();
			services.AddScoped<UserService>();
			return services;
		}
	}
}