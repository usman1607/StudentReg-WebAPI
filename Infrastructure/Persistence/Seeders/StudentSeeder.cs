using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seeders
{
    public static class StudentSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

            try
            {
                // Check if data already exists
                if (await context.Students.AnyAsync())
                {
                    logger.LogInformation("Database already seeded. Skipping seeding.");
                    return;
                }

                logger.LogInformation("Seeding database with initial student data...");

                var students = new List<Student>
                {
                    new Student(
                        matricNumber: "STU/2024/10001",
                        firstName: "John",
                        lastName: "Doe",
                        email: "john.doe@university.edu",
                        passwordHash: "hashed_password_1",
                        hashSalt: "salt_1",
                        gender: Domain.Enums.Gender.Male,
                        phoneNumber: "+234801234567",
                        address: "123 University Road, Lagos",
                        createdBy: "System"
                    )
                    
                };

                await context.Students.AddRangeAsync(students);
                await context.SaveChangesAsync();

                logger.LogInformation("Successfully seeded {Count} students.", students.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}
