using Application.Configurations;
using Application.Mappings;
using Application.Repositories;
using Application.Services.Contracts;
using Application.Services.Implementations;
using Application.Services.Interfaces;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.FileStorage;
using MassTransit;
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
            services.AddScoped<IStaffRepository, StaffRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IStudentsCoursesRepository, StudentsCoursesRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            // Application Services
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IPaymentService, PaymentService>();

            services.AddHttpClient<IPaystackService, PaystackService>();

            services.AddSingleton<IFileServiceFactory, FileServiceFactory>();
            services.AddTransient<CloudinaryFileService>();
            services.AddTransient<LocalFileService>();
            services.AddTransient<AWSFileService>();            

            // Add Configuration options
            services.Configure<MailConfig>(configuration.GetSection("MailConfig"));
            services.Configure<StorageSettings>(configuration.GetSection("StorageSettings"));
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.Configure<PaystackSettings>(configuration.GetSection("PaystackSettings"));
            services.Configure<BankAccountSettings>(configuration.GetSection("BankAccountSettings"));

            // Add Hangfire for background jobs
            services.AddHangfire(config =>
                        config.UsePostgreSqlStorage(
                            configuration.GetConnectionString("DefaultConnection")
                        ));

            services.AddHangfireServer();

            services.AddScoped<IBackgroundJobs, MyBackgroundJobs>();

            // AutoMapper
            services.AddAutoMapper(typeof(StudentMappingProfile).Assembly);

            // MassTransit + RabbitMQ
            services.AddMassTransit(x =>
            {
                x.AddConsumer<StudentRegisteredConsumer>();
                x.AddConsumer<StudentAdmissionAcceptedConsumer>();
                x.AddConsumer<CourseAssignedConsumer>();

                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(
                        configuration["RabbitMQ:Host"] ?? "localhost",
                        configuration["RabbitMQ:VirtualHost"] ?? "/",
                        h =>
                        {
                            h.Username(configuration["RabbitMQ:Username"] ?? "guest");
                            h.Password(configuration["RabbitMQ:Password"] ?? "guest");
                        });

                    cfg.ConfigureEndpoints(ctx);
                });
            });

            services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();

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