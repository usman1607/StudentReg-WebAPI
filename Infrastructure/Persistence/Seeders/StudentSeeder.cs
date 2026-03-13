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
                // Ensure database is created and migrations applied
                await context.Database.MigrateAsync();

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
                        matricNo: "STU/2024/10001",
                        firstName: "John",
                        lastName: "Doe",
                        email: "john.doe@university.edu",
                        passwordHash: "hashed_password_1",
                        hashSalt: "salt_1",
                        phoneNo: "+234801234567",
                        address: "123 University Road, Lagos",
                        createdBy: "System"
                    ),
                    new Student(
                        matricNo: "STU/2024/10002",
                        firstName: "Jane",
                        lastName: "Smith",
                        email: "jane.smith@university.edu",
                        passwordHash: "hashed_password_2",
                        hashSalt: "salt_2",
                        phoneNo: "+234802345678",
                        address: "456 College Avenue, Abuja",
                        createdBy: "System"
                    ),
                    new Student(
                        matricNo: "STU/2024/10003",
                        firstName: "Michael",
                        lastName: "Johnson",
                        email: "michael.johnson@university.edu",
                        passwordHash: "hashed_password_3",
                        hashSalt: "salt_3",
                        phoneNo: "+234803456789",
                        address: "789 Campus Drive, Port Harcourt",
                        createdBy: "System"
                    ),
                    new Student(
                        matricNo: "STU/2024/10004",
                        firstName: "Emily",
                        lastName: "Brown",
                        email: "emily.brown@university.edu",
                        passwordHash: "hashed_password_4",
                        hashSalt: "salt_4",
                        phoneNo: "+234804567890",
                        address: "321 Education Street, Ibadan",
                        createdBy: "System"
                    ),
                    new Student(
                        matricNo: "STU/2024/10005",
                        firstName: "David",
                        lastName: "Wilson",
                        email: "david.wilson@university.edu",
                        passwordHash: "hashed_password_5",
                        hashSalt: "salt_5",
                        phoneNo: "+234805678901",
                        address: "654 Learning Lane, Kano",
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
