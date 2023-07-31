namespace AuthenticationService.Security;

using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AuthenticationService.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class ServiceAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly AuthenticationServiceOptions _authenticationOptions;
    public ServiceAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> authenticationSchemeOptions,
                                        ILoggerFactory logger,
                                        UrlEncoder encoder,
                                        ISystemClock clock,
                                        IOptionsMonitor<AuthenticationServiceOptions> authenticationOptions) : base(authenticationSchemeOptions, logger, encoder, clock)
    {
        _authenticationOptions = authenticationOptions.CurrentValue;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Api-Key", out var apiKeyValues))
        {
            return Task.FromResult(AuthenticateResult.Fail("No API key provided."));
        }

        var apiKey = apiKeyValues.ToString();
        if (string.Equals(apiKey, _authenticationOptions.ApiKey, StringComparison.Ordinal))
        {
            var claims = new[]
            {
                new Claim("ServiceClaim", "true"),
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
    }
}
