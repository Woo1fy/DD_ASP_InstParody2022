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
		// �������������� ����� ��������� ������ WebApplicationBuilder � �������������� ������������ ���������� �� ���������.
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddConfig(builder.Configuration).AddDependencyGroup();

		var app = builder.Build();

		// ������ ���, ����� ��� ������ ������� ����������� �������� (����������� ������)
		// ������� scoped ����� � ������ �������, ����� ����������� ���� Instance, ���������� �� ���-�� ���������
		using (var serviceScope = ((IApplicationBuilder)app).ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope()) {
			if (serviceScope != null) {
				// �������� ������ ��������� � DAL
				var context = serviceScope.ServiceProvider.GetRequiredService<DAL.DataContext>();
				context.Database.Migrate();
			}
		}

		// Configure the HTTP request pipeline.

		// ������������� �� ��� ������������ ���������������� ��������� JSON � ����������������� ���������� Swagger

		//if (app.Environment.IsDevelopment()) // ����� �������� ������ � ������ "����������"
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