namespace AuthenticationService.Backend.Security;

using AuthenticationService.Backend.Options;
using Microsoft.Extensions.Options;

public interface IPasswordHasherService
{
    /// <summary>
    /// Hashes a password
    /// </summary>
    /// <param name="plainPassword">Plain password</param>
    /// <returns></returns>
    string HashPassword(string plainPassword);

    /// <summary>
    /// Verifies that the hashed password is correct
    /// </summary>
    /// <param name="hashedPassword">hashed password</param>
    /// <param name="plainPassword">plain password</param>
    /// <returns></returns>
    bool VerifyPassword(string hashedPassword, string plainPassword);
}

public class PasswordHasherService : IPasswordHasherService
{
    public string HashPassword(string plainPassword)
    {
        string salt = "$2a$12$hG5lf/9xPbovhz8kATDgd.";
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword, salt);
        return hashedPassword;
    }

    public bool VerifyPassword(string hashedPassword, string plainPassword)
    {
        bool passwordMatches = BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);

        return passwordMatches;
        // Decode the base64-encoded hash
        //byte[] hashBytes = Convert.FromBase64String(hashedPassword);
        //var validatePassword = BCrypt.EnhancedVerify(myPassword, enhancedHashPassword);

        //// Hash the input password with the same parameters
        //using (var argon2 = new Argon2id(password))
        //{
        //    argon2.Salt = hashBytes[..16]; // Extract the salt from the stored hash
        //    argon2.Iterations = 1_024;
        //    argon2.MemorySize = 64 * 1024; // 64MB
        //    argon2.DegreeOfParallelism = 4; // 4 threads
        //    argon2.OutputSize = 32; // 256 bits

        //    // Compare the hash of the input password with the stored hash
        //    byte[] inputHash = argon2.GetBytes();
        //    return StructuralComparisons.StructuralEqualityComparer.Equals(inputHash, hashBytes);
        //}
    }
}
