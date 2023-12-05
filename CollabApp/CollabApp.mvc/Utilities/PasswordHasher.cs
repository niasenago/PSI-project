using System.Security.Cryptography;
using System.Text;

public static class PasswordHasher
{
    public static string GenerateSalt()
    {
        byte[] salt = new byte[32];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }
        return Convert.ToBase64String(salt);
    }

    public static string HashPassword(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            var saltedPassword = Encoding.UTF8.GetBytes(password + salt);
            var hashedPassword = sha256.ComputeHash(saltedPassword);
            return Convert.ToBase64String(hashedPassword);
        }
    }
}
