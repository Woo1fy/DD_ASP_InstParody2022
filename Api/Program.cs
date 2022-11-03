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

		builder.Services.AddConfig(builder.Configuration).AddDependencyGroup();

		var app = builder.Build();

		// Делаем так, чтобы при каждом запуске запускалась миграции (обновлялись данные)
		// Сервисы scoped живут в рамках запроса, будет использован один Instance, независимо от кол-ва обращений
		using (var serviceScope = ((IApplicationBuilder)app).ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope()) {
			if (serviceScope != null) {
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
		app.UseTokenValidator();
        app.MapControllers();

        app.Run();
    }
}