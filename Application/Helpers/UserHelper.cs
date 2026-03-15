using Domain.Enums;
using System.Reflection.Metadata.Ecma335;

namespace Application.Helpers
{
    public static class UserHelper
    {
        public static (string hash, string salt) GeneratePasswordHash(string password)
        {
            var salt = Guid.NewGuid().ToString("N")[..16];
            var hash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes($"{password}{salt}")));
            return (hash, salt);
        }

        public static string GenerateStaffNumber(StaffDelegation delegation)
        {
            var prefix = delegation switch
            {
                StaffDelegation.Admin => "ADM",
                StaffDelegation.Instructor => "INS",
                StaffDelegation.Registrar => "REG",
                StaffDelegation.AcademicAdvisor => "ADV",
                _ => "STF"
            };

            var year = DateTime.UtcNow.Year;
            var random = new Random().Next(10000, 99999);
            return $"{prefix}/{year}/{random}";
        }

        public static string GenerateMatricNumber()
        {
            var year = DateTime.UtcNow.Year;
            var random = new Random().Next(10000, 99999);
            return $"STU/{year}/{random}";
        }

        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            var hash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes($"{password}{storedSalt}")));
            return hash == storedHash;
        }
    }
}
