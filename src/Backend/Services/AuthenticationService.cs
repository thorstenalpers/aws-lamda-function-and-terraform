namespace AuthenticationService.Backend.Services;

using global::AuthenticationService.Backend.Repositories;
using global::AuthenticationService.Backend.Security;

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
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly ICredentialRepository _credentialRepository;

    public AuthenticationService(ILogger<AuthenticationService> logger,
                                    ICredentialRepository credentialRepository,
                                    IPasswordHasherService passwordHasherService)
    {
        _logger = logger;
        _passwordHasherService = passwordHasherService;
        _credentialRepository = credentialRepository;
    }

    public async Task<bool> AreCredentialsValidAsync(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return false;
        try
        {
            var credentials = await _credentialRepository.GetCredentialsAsync(username);
            var isPasswordValid = _passwordHasherService.VerifyPassword(credentials.Password, password);
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
}
