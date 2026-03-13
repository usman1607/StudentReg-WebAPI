using Application.Mappings;
using Application.Repositories;
using Application.Services.Implementations;
using Application.Services.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDatabase(configuration);

            // Repositories
            services.AddScoped<IStudentRepository, StudentRepository>();

            // Application Services
            services.AddScoped<IStudentService, StudentService>();

            // AutoMapper
            services.AddAutoMapper(typeof(StudentMappingProfile).Assembly);

            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            return services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString, action => action.MigrationsAssembly("Infrastructure")));
        }
    }
}