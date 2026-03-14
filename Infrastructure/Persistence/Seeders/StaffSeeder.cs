using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seeders
{
    public static class StaffSeeder
    {
        public static readonly Guid AdminStaffId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

            try
            {
                var adminExists = await context.Staff.AnyAsync(s => s.Id == AdminStaffId);
                if (!adminExists)
                {
                    // Generate password hash for "Admin@123"
                    var (hash, salt) = GeneratePasswordHash("Admin@123");

                    var adminStaff = new Staff(
                        staffNumber: "ADM/2024/00001",
                        firstName: "System",
                        lastName: "Administrator",
                        email: "admin@studentreg.com",
                        passwordHash: hash,
                        hashSalt: salt,
                        phoneNumber: "0000000000",
                        address: "System",
                        delegation: StaffDelegation.Admin,
                        createdBy: "System"
                    );

                    // Set the specific ID
                    typeof(BaseEntity).GetProperty("Id")!.SetValue(adminStaff, AdminStaffId);

                    context.Staff.Add(adminStaff);

                    // Assign Admin role
                    var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Id == RoleSeeder.AdminRoleId);
                    if (adminRole != null)
                    {
                        var userRole = new UserRole
                        {
                            UserId = AdminStaffId,
                            RoleId = RoleSeeder.AdminRoleId,
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

        private static (string hash, string salt) GeneratePasswordHash(string password)
        {
            var salt = Guid.NewGuid().ToString("N")[..16];
            var hash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes($"{password}{salt}")));
            return (hash, salt);
        }
    }
}
