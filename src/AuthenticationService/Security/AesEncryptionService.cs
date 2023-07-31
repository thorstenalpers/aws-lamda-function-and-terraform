namespace AuthenticationService.Security;

using System.Security.Cryptography;
using System.Text;
using System;
using Microsoft.Extensions.Options;
using AuthenticationService.Exceptions;
using AuthenticationService.Options;

public interface IAesEncryptionService
{
    /// <summary>
    /// Encrypts plain text
    /// </summary>
    /// <param name="plainText">text to encrypt</param>
    /// <returns>encrypted text</returns>
    /// <exception cref="AuthenticatorException"></exception>
    string EncryptString(string plainText);

    /// <summary>
    /// Decrypts an encrypted text
    /// </summary>
    /// <param name="encryptedText">text to decrypt</param>
    /// <returns>decrypted text</returns>
    string DecryptString(string encryptedText);
}

public class AesEncryptionService : IAesEncryptionService
{
    private readonly AuthenticationServiceOptions _authenticatorOptions;

    public AesEncryptionService(IOptionsSnapshot<AuthenticationServiceOptions> authenticatorOptions)
    {
        _authenticatorOptions = authenticatorOptions.Value;
    }

    public string EncryptString(string plainText)
    {
        byte[] key = Convert.FromBase64String(_authenticatorOptions.CipherKey); // 32 Bit, 256-bit key for AES-256
        byte[] iv = Convert.FromBase64String(_authenticatorOptions.CipherIv);  // 16 Bit, 128-bit IV for AES

        if (key.Length != 32) throw new AuthenticatorException("CipherKey is invalid");
        if (iv.Length != 16) throw new AuthenticatorException("CipherIv is invalid");

        using Aes aes = Aes.Create();

        aes.Key = key;
        aes.IV = iv;

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        return Convert.ToBase64String(encryptedBytes);
    }

    public string DecryptString(string encryptedText)
    {
        byte[] key = Convert.FromBase64String(_authenticatorOptions.CipherKey); // 32 Bit, 256-bit key for AES-256
        byte[] iv = Convert.FromBase64String(_authenticatorOptions.CipherIv);  // 16 Bit, 128-bit IV for AES

        if (key.Length != 32) throw new AuthenticatorException("CipherKey is invalid");
        if (iv.Length != 16) throw new AuthenticatorException("CipherIv is invalid");

        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

        using Aes aes = Aes.Create();

        aes.Key = key;
        aes.IV = iv;

        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
        var decryptedText = Encoding.UTF8.GetString(decryptedBytes);

        return decryptedText;
    }
}
