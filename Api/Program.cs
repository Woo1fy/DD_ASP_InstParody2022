using Api;
using Api.Configs;
using Api.Services;
using Api.Services.Abstract;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

internal class Program
{
    private static void Main(string[] args)
    {
		// Инициализирует новый экземпляр класса WebApplicationBuilder с предварительно настроенными значениями по умолчанию.
		var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Создаем конфигурацию для аутентификации
        var authSection = builder.Configuration.GetSection(AuthConfig.Position);

		/// <summary>
		/// Попытка связать экземпляр конфигурации с новым экземпляром типа T.
		/// Если у этого раздела конфигурации есть значение, оно будет использовано.
		/// В противном случае привязка осуществляется путем рекурсивного сопоставления имен свойств с ключами конфигурации.
		/// </summary>
		/// <typeparam name="T">Тип нового экземпляра для связывания.</typeparam>
		/// <param name="configuration">Экземпляр конфигурации для связывания.</param>
		/// <returns>Новый экземпляр T в случае успеха, default(T) в противном случае.</returns>
		var authConfig = authSection.Get<AuthConfig>();

        // Регистрируем конфигурацию
        builder.Services.Configure<AuthConfig>(authSection);

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        // Добавляю генератор Swagger в сервисы
        builder.Services.AddSwaggerGen(c =>
        {
            // Добавляю дополнительные элементы для отображения в пользовательском интерфейсе Swagger
			c.SwaggerDoc("v1", new OpenApiInfo {
				Version = "v1",
				Title = "ToDo API",
				Description = "An ASP.NET Core Web API for managing ToDo items",
				TermsOfService = new Uri("https://example.com/terms"),
				Contact = new OpenApiContact {
					Name = "Example Contact",
					Url = new Uri("https://example.com/contact")
				},
				License = new OpenApiLicense {
					Name = "Example License",
					Url = new Uri("https://example.com/license")
				}
			});


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

        // Регистрируем для DAL базу данных (чтобы видел :) )
        builder.Services.AddDbContext<DAL.DataContext>(options =>
        {
            // Указываем, что мы используем для подключения
            // Для connectionString есть свой обработчик, вон он снизу 
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"), sql => { });
        });


        builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);

        builder.Services.AddScoped<IUserService, UserService>();
		builder.Services.AddScoped<ITokenService, TokenService>();

		builder.Services.AddAuthentication(o =>
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

        builder.Services.AddAuthorization(o =>
        {
            o.AddPolicy("ValidAccessToken", p =>
            {
                p.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                p.RequireAuthenticatedUser();
            });
        });

        var app = builder.Build();

        // Делаем так, чтобы при каждом запуске запускалась миграции (обновлялись данные)
        // Сервисы scoped живут в рамках запроса, будет использован один Instance, независимо от кол-ва обращений
        using (var serviceScope = ((IApplicationBuilder)app).ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
        {
            if (serviceScope != null)
            {
                // Получаем сервис контекста с DAL
                var context = serviceScope.ServiceProvider.GetRequiredService<DAL.DataContext>();
                context.Database.Migrate();
            }
        }

		// Configure the HTTP request pipeline.

		// Промежуточное ПО для обслуживания сгенерированного документа JSON и пользовательского интерфейса Swagger

		//if (app.Environment.IsDevelopment()) // Будет работать только в режиме "Разработка"
		{
			app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}