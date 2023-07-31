namespace AuthenticationService.Services;

using Amazon.DynamoDBv2.DataModel;
using global::AuthenticationService.Exceptions;
using global::AuthenticationService.Models;

public interface IAuthenticationService
{
    /// <summary>
    /// Verifies that the credentials are valid and stored in the DB
    /// </summary>
    /// <param name="encryptedUsername">encrypted username</param>
    /// <param name="encryptedPassword">encrypted password</param>
    /// <returns></returns>
    Task<bool> AreCredentialsValidAsync(string encryptedUsername, string encryptedPassword);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly IDynamoDBContext _dynamoDBContext;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(IDynamoDBContext dynamoDBContext,
    ILogger<AuthenticationService> logger)
    {
        _logger = logger;
        _dynamoDBContext = dynamoDBContext;
    }

    public async Task<bool> AreCredentialsValidAsync(string encryptedUsername, string encryptedPassword)
    {
        if (string.IsNullOrEmpty(encryptedUsername) || string.IsNullOrEmpty(encryptedPassword))
            return false;
        try
        {
            // Get the user's record from DynamoDB
            var credentials = await GetCredentials(encryptedUsername);

            bool isPasswordValid = string.Equals(encryptedPassword, credentials.encryptedPassword, StringComparison.Ordinal);
            if (!isPasswordValid)
            {
                _logger.LogWarning($"Password of {encryptedUsername} invalid and not equal as in DB");
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

    private async Task<(string encryptedUsername, string encryptedPassword)> GetCredentials(string encryptedUsername)
    {
        var credential = await _dynamoDBContext.LoadAsync<Credential>(encryptedUsername);
        if (credential?.Password == null)
            throw new AuthenticatorException($"No record found for {encryptedUsername}");

        return (encryptedUsername, credential.Password);
    }
}
