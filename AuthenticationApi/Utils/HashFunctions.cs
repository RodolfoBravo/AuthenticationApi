using System;
using System.Security.Cryptography;
using System.Text;

namespace AuthenticationApi.Utils
{
    public class HashFunctions
    {
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
        public static bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            using var sha256 = SHA256.Create();
            var enteredPasswordHash = HashPassword(enteredPassword);

            return enteredPasswordHash == hashedPassword;
        }
    }
}
