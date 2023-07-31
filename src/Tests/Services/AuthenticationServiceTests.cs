namespace AuthenticationService.Tests.Services;

using Amazon.DynamoDBv2.DataModel;
using AuthenticationService.Models;
using AuthenticationService.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;


[TestFixture]
[Category("UnitTests")]
public class AuthenticationServiceTests
{
    private MockRepository _mockRepository;
    private Mock<IDynamoDBContext> _mockDynamoDBContext;
    private Mock<ILogger<AuthenticationService>> _mockLogger;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new MockRepository(MockBehavior.Strict);

        _mockDynamoDBContext = _mockRepository.Create<IDynamoDBContext>();
        _mockLogger = new Mock<ILogger<AuthenticationService>>();
    }

    private AuthenticationService CreateService()
    {
        return new AuthenticationService(
            _mockDynamoDBContext.Object,
            _mockLogger.Object);
    }

    [Test]
    public async Task AreCredentialsValidAsync_Success()
    {
        // Arrange
        var service = CreateService();
        var credential = new Credential
        {
            Username = "encryptedUsername",
            Password = "encryptedPassword"
        };
        _mockDynamoDBContext.Setup(x => x.LoadAsync<Credential>(credential.Username, default)).ReturnsAsync(credential);

        // Act
        var valid = await service.AreCredentialsValidAsync(
            credential.Username,
            credential.Password);

        // Assert
        Assert.True(valid);
    }

    [TestCase("encryptedUsername", "", "encryptedUsername", "encryptedPassword")]
    [TestCase("", "encryptedPassword", "encryptedUsername", "encryptedPassword")]
    [TestCase("", "", "encryptedUsername", "encryptedPassword")]
    [TestCase("encryptedUsername", "encryptedPassword", "encryptedUsername", "WrongPassword")]
    public async Task AreCredentialsValidAsync_Fail(string encryptedUsername, string encryptedPassword, string storedUsername, string storedPassword)
    {
        // Arrange
        var service = CreateService();
        _mockDynamoDBContext.Setup(x => x.LoadAsync<Credential>(encryptedUsername, default)).ReturnsAsync(
            new Credential
            {
                Username = storedUsername,
                Password = storedPassword
            });

        // Act
        var valid = await service.AreCredentialsValidAsync(
            encryptedUsername,
            encryptedPassword);

        // Assert
        Assert.False(valid);
    }
}
