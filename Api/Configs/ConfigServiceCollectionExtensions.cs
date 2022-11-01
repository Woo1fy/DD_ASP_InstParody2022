using Api.Services.Abstract;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Api.Configs {
	public static class ConfigServiceCollectionExtensions {

		public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration config) {

			// Add services to the container.
			// Создаем конфигурацию для аутентификации
			var authSection = config.GetSection(AuthConfig.Position);
			var authConfig = authSection.Get<AuthConfig>();

			services.Configure<AuthConfig>(authSection);

			services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			services.AddEndpointsApiExplorer();

			// Добавляю генератор Swagger в сервисы
			services.AddSwaggerGen(c =>
			{

				c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme {
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

			// Регистрируем для DAL базу данных (чтобы видел :) )
			services.AddDbContext<DAL.DataContext>(options =>
			{
				// Указываем, что мы используем для подключения
				// Для connectionString есть свой обработчик, вон он снизу 
				options.UseNpgsql(config.GetConnectionString("PostgreSql"), sql => { });
			});


			services.AddAutoMapper(typeof(MapperProfile).Assembly);



			services.AddAuthentication(o =>
			{
				o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(o =>
			{
				o.RequireHttpsMetadata = false;
				o.TokenValidationParameters = new TokenValidationParameters {
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

		public static IServiceCollection AddDependencyGroup(this IServiceCollection services) {

			services.AddScoped<IUserService, UserService>();
			services.AddScoped<ITokenService, TokenService>();
			return services;
		}
	}
}
