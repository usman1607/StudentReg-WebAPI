using Domain.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seeders
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

            try
            {
                if (!await context.Roles.AnyAsync())
                {
                    var roles = new List<Role>
                    {
                        new Role
                        {
                            Id = Guid.NewGuid(),
                            Name = RoleNames.Admin,
                            Description = "Administrator with full access",
                            CreatedBy = "System"
                        },
                        new Role
                        {
                            Id = Guid.NewGuid(),
                            Name = RoleNames.Student,
                            Description = "Student user",
                            CreatedBy = "System"
                        },
                        new Role
                        {
                            Id = Guid.NewGuid(),
                            Name = RoleNames.Instructor,
                            Description = "Instructor who can assign courses",
                            CreatedBy = "System"
                        }
                    };
                    context.Roles.AddRange(roles);
                    logger.LogInformation("Seeding roles: {RoleName}", string.Join(", ", roles));                        
                    await context.SaveChangesAsync();
                    logger.LogInformation("Role seeding completed");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error seeding roles");
                throw;
            }
        }
    }
}
