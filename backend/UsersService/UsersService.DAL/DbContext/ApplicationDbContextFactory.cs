using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace UsersService.DAL.DbContext
{
    // Цей клас використовується лише EF Core tools для міграцій.
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            
            // !!!! ВАЖЛИВО: Використовуйте рядок підключення з вашого appsettings.json !!!!
            // Цей рядок повинен бути дійсним, щоб EF зміг підключитися.
            // Приклад для локального PostgreSQL:
            const string containerHostName = "flower-lab-postgres"; // або просто "postgres"
        
            // Використовуй значення, які ти вводив у Aspire Dashboard
            const string connectionString = $"Host={containerHostName};Port=5432;Database=UsersDB;Username=postgres;Password=password";

            optionsBuilder.UseNpgsql(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}