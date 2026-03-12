using Application.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var connectionString = "Host=localhost;Port=5432;Database=smsdb;Username=postgres;Password=Oluwatobiloba007;";

            return serviceCollection
                .AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(connectionString, action => action.MigrationsAssembly("Infrastructure")))
                .AddScoped<IStudentRepository, StudentRepository>();

        }

        
    }
}