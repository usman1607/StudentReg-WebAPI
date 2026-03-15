using Application.Helpers;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seeders
{
    public static class StaffSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

            try
            {
                if (!await context.Staff.AnyAsync())
                {
                    // Generate password hash for "Admin@123"
                    var (hash, salt) = UserHelper.GeneratePasswordHash("Admin@123");

                    var adminStaff = new Staff(
                        staffNumber: "ADM/2024/00001",
                        firstName: "System",
                        lastName: "Administrator",
                        email: "admin@studentreg.com",
                        passwordHash: hash,
                        hashSalt: salt,
                        gender: Gender.Male,
                        phoneNumber: "09088776654",
                        address: "System",
                        delegation: StaffDelegation.Admin,
                        createdBy: "System"
                    );

                    context.Staff.Add(adminStaff);

                    // Assign Admin role
                    var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == RoleNames.Admin);
                    if (adminRole != null)
                    {
                        var userRole = new UserRole
                        {
                            UserId = adminStaff.Id,
                            RoleId = adminRole.Id,
                            CreatedBy = "System"
                        };
                        context.UserRoles.Add(userRole);
                    }

                    await context.SaveChangesAsync();
                    logger.LogInformation("Admin staff seeded successfully. Email: admin@studentreg.com, Password: Admin@123");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error seeding admin staff");
                throw;
            }
        }
    }
}
