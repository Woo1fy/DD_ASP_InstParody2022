using Api.Configs;
using Microsoft.EntityFrameworkCore;

namespace Api
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddConfig(builder.Configuration).AddServicesGroup();

			var app = builder.Build();

			using (var serviceScope = ((IApplicationBuilder)app).ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
			{
				if (serviceScope != null)
				{
					var context = serviceScope.ServiceProvider.GetRequiredService<DAL.DataContext>();
					context.Database.Migrate();
				}
			}

			// Configure the HTTP request pipeline.
			//if (app.Environment.IsDevelopment())
			app.UseSwagger();
			app.UseSwaggerUI();

			app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();
			app.UseTokenValidator();
			app.MapControllers();

			app.Run();
		}
	}
}