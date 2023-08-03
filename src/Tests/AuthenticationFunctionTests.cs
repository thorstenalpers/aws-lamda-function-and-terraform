namespace AuthenticationService.Tests;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using AuthenticationService.Backend;
using AuthenticationService.Backend.Security;
using AuthenticationService.Backend.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

[TestFixture]
[Category("UnitTests")]
public class AuthenticationFunctionTests
{
    private MockRepository _mockRepository;
    private Mock<ILogger<AuthenticationFunction>> _mockLogger;
    private Mock<IAuthenticationService> _mockAuthenticationService;
    private Mock<ILambdaContext> _mockLambdaContext;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new MockRepository(MockBehavior.Strict);

        _mockAuthenticationService = _mockRepository.Create<IAuthenticationService>();
        _mockLambdaContext = _mockRepository.Create<ILambdaContext>();
        _mockLogger = new Mock<ILogger<AuthenticationFunction>>();
    }

    [Test]
    public async Task AuthenticationFunction_ValidCredentials_Success()
    {
        // Arrange
        _mockAuthenticationService.Setup(dependency => dependency
            .AreCredentialsValidAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.FromResult(true));

        var lambdaFunction = new AuthenticationFunction(_mockLogger.Object, _mockAuthenticationService.Object);
        var request = CreateValidRequest();

        // Act
        var response = await lambdaFunction.AuthenticateAsync(request, _mockLambdaContext.Object);

        // Assert
        Assert.AreEqual(200, response.StatusCode);
    }

    [Test]
    public async Task AuthenticationFunction_InvalidCredentials_Success()
    {
        // Arrange
        _mockAuthenticationService.Setup(dependency => dependency
            .AreCredentialsValidAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.FromResult(false));

        var lambdaFunction = new AuthenticationFunction(_mockLogger.Object, _mockAuthenticationService.Object);
        var request = CreateValidRequest();

        // Act
        var response = await lambdaFunction.AuthenticateAsync(request, _mockLambdaContext.Object);

        // Assert
        Assert.AreEqual(401, response.StatusCode);
        _mockLogger.VerifyLog(logger => logger.LogWarning("Unauthorized: Credentials invalid or not found"), Times.Once);
    }

    private static APIGatewayProxyRequest CreateValidRequest()
    {
        var aesEncryptionService = new PasswordHasherService();
        var username = "Username";
        var password = "Password";
        var request = new APIGatewayProxyRequest
        {
            Headers = new Dictionary<string, string>
        {
            { "Authorization", $"Basic {username}:{password}" }
        }
        };
        return request;
    }
}
