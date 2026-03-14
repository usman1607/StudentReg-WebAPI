using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seeders
{
    public static class RoleSeeder
    {
        public static readonly Guid AdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid StudentRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public static readonly Guid InstructorRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

            try
            {
                var roles = new List<Role>
                {
                    new Role
                    {
                        Id = AdminRoleId,
                        Name = "Admin",
                        Description = "Administrator with full access",
                        CreatedBy = "System"
                    },
                    new Role
                    {
                        Id = StudentRoleId,
                        Name = "Student",
                        Description = "Student user",
                        CreatedBy = "System"
                    },
                    new Role
                    {
                        Id = InstructorRoleId,
                        Name = "Instructor",
                        Description = "Instructor who can assign courses",
                        CreatedBy = "System"
                    }
                };

                foreach (var role in roles)
                {
                    var exists = await context.Roles.AnyAsync(r => r.Id == role.Id);
                    if (!exists)
                    {
                        context.Roles.Add(role);
                        logger.LogInformation("Seeding role: {RoleName}", role.Name);
                    }
                }

                await context.SaveChangesAsync();
                logger.LogInformation("Role seeding completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error seeding roles");
                throw;
            }
        }
    }
}
