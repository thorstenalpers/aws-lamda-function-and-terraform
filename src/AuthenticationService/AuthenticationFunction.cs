using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AuthenticationService;

using System;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Authorization;

public class AuthenticationFunction
{
    private readonly ILogger<AuthenticationFunction> _logger;
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationFunction(ILogger<AuthenticationFunction> logger,
        IAuthenticationService authenticationService)
    {
        _logger = logger;
        _authenticationService = authenticationService;
    }

    /// <summary>
    /// Validates credentials
    /// </summary>
    /// <param name="request">HttpHeader with Basic Authentication base64 encoded and encrypted</param>
    /// <param name="context"></param>
    /// <returns>Status Code 200 on success, otherwise 401 </returns>
    [Authorize]
    public async Task<APIGatewayProxyResponse> AuthenticateAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        try
        {
            if (!request.Headers.TryGetValue("Authorization", out var authorizationHeader) ||
                !authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                return GenerateUnauthorizedResponse("Basic authentication credentials missing");
            }
            var encodedCredentials = authorizationHeader.Substring("Basic ".Length).Trim();
            var separatorIndex = encodedCredentials.IndexOf(':');
            if (separatorIndex == -1)
            {
                return GenerateUnauthorizedResponse("Invalid basic authentication credentials");
            }
            var encryptedUsername = encodedCredentials.Substring(0, separatorIndex);
            var encryptedPassword = encodedCredentials.Substring(separatorIndex + 1);

            var areCredentialsValid = await _authenticationService.AreCredentialsValidAsync(encryptedUsername, encryptedPassword);
            if (!areCredentialsValid)
            {
                return GenerateUnauthorizedResponse("Credentials invalid or not found");
            }
            return GenerateSuccessResponse("Authenticated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during authentication: {ex.Message}");
            return GenerateErrorResponse("Internal server error");
        }
    }

    private APIGatewayProxyResponse GenerateUnauthorizedResponse(string message)
    {
        _logger.LogWarning($"Unauthorized: " + message);
        return new APIGatewayProxyResponse
        {
            StatusCode = 401,
            Body = message
        };
    }

    private static APIGatewayProxyResponse GenerateSuccessResponse(string message)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = message
        };
    }

    private static APIGatewayProxyResponse GenerateErrorResponse(string message)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 500,
            Body = message
        };
    }
}
