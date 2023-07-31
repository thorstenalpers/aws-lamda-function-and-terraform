namespace AuthenticationService.Services;

using Amazon.DynamoDBv2.DataModel;
using global::AuthenticationService.Exceptions;
using global::AuthenticationService.Models;
using global::AuthenticationService.Security;

public interface IAuthenticationService
{
    /// <summary>
    /// Verifies that the credentials are valid and stored in the DB
    /// </summary>
    /// <param name="username">plain username</param>
    /// <param name="password">plain password</param>
    /// <returns></returns>
    Task<bool> AreCredentialsValidAsync(string username, string password);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IDynamoDBContext _dynamoDBContext;
    private readonly IAesEncryptionService _aesEncryptionService;

    public AuthenticationService(ILogger<AuthenticationService> logger,
                                    IDynamoDBContext dynamoDBContext,
                                    IAesEncryptionService aesEncryptionService)
    {
        _logger = logger;
        _dynamoDBContext = dynamoDBContext;
        _aesEncryptionService = aesEncryptionService;
    }

    public async Task<bool> AreCredentialsValidAsync(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return false;
        try
        {
            var credentials = await GetCredentials(username);
            var encryptedPassword = _aesEncryptionService.EncryptString(password);
            bool isPasswordValid = string.Equals(encryptedPassword, credentials.Password, StringComparison.Ordinal);
            if (!isPasswordValid)
            {
                _logger.LogWarning($"Password of {username} invalid and not present in DB");
                return false;
            }
            return isPasswordValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message}");
            return false;
        }
    }

    private async Task<Credential> GetCredentials(string username)
    {
        var encryptedUsername = _aesEncryptionService.EncryptString(username);
        var credential = await _dynamoDBContext.LoadAsync<Credential>(encryptedUsername);
        if (credential?.Password == null)
            throw new AuthenticatorException($"No record found for {username}");

        return credential;
    }
}
