using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        // 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>()
                .HasIndex(f => f.Email)
                .IsUnique();
        }

		/// <summary>
		///  Настраивает контекст для подключения к серверу PostgreSQL с помощью Npgsql, но без первоначальной установки System.Data.Common.DbConnection или строки подключения. 
        ///  Соединение или строка соединения должны быть установлены до использования Microsoft.EntityFrameworkCore.DbContext для подключения к базе данных. 
		/// </summary>
		/// <param name="optionsBuilder"> переопределнные параметры настройки DbContext</param>
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(b => b.MigrationsAssembly("Api"));

		/// <summary>
		/// DbSet представляет собой набор всех сущностей в контексте или которые могут быть запрошены из базы данных данного типа.
		/// Объекты DbSet создаются из DbContext с помощью метода DbContext.Set.
		/// </summary>
		public DbSet<User> Users => Set<User>();
    }
}
