namespace AuthenticationService.Tests.Services;

using Amazon.DynamoDBv2.DataModel;
using AuthenticationService.Models;
using AuthenticationService.Security;
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
    private Mock<IAesEncryptionService> _mockAesEncryptionService;
    private Mock<ILogger<AuthenticationService>> _mockLogger;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new MockRepository(MockBehavior.Strict);

        _mockDynamoDBContext = _mockRepository.Create<IDynamoDBContext>();
        _mockAesEncryptionService = _mockRepository.Create<IAesEncryptionService>();
        _mockLogger = new Mock<ILogger<AuthenticationService>>();
    }

    private AuthenticationService CreateService()
    {
        return new AuthenticationService(
            _mockLogger.Object,
            _mockDynamoDBContext.Object,
            _mockAesEncryptionService.Object);
    }

    [Test]
    public async Task AreCredentialsValidAsync_Success()
    {
        // Arrange
        var service = CreateService();
        var plainCredential = new Credential
        {
            Username = "username",
            Password = "password"
        };
        var encryptedCredential = new Credential
        {
            Username = "encryptedUsername",
            Password = "encryptedPassword"
        };
        _mockDynamoDBContext.Setup(x => x.LoadAsync<Credential>(encryptedCredential.Username, default)).ReturnsAsync(encryptedCredential);
        _mockAesEncryptionService.Setup(e => e.EncryptString(plainCredential.Username)).Returns(encryptedCredential.Username);
        _mockAesEncryptionService.Setup(e => e.EncryptString(plainCredential.Password)).Returns(encryptedCredential.Password);

        // Act
        var valid = await service.AreCredentialsValidAsync(
            plainCredential.Username,
            plainCredential.Password);

        // Assert
        Assert.True(valid);
    }

    [TestCase("plainUsername", "", "encryptedUsername", "encryptedPassword")]
    [TestCase("", "plainPassword", "encryptedUsername", "encryptedPassword")]
    [TestCase("", "", "encryptedUsername", "encryptedPassword")]
    [TestCase("plainUsername", "plainPassword", "encryptedUsername", "WrongPassword")]
    public async Task AreCredentialsValidAsync_Fail(string plainUsername, string plainPassword, string storedUsername, string storedPassword)
    {
        // Arrange
        var service = CreateService();

        _mockDynamoDBContext.Setup(x => x.LoadAsync<Credential>(storedUsername, default)).ReturnsAsync(
            new Credential
            {
                Username = storedUsername,
                Password = storedPassword
            });
        _mockAesEncryptionService.Setup(e => e.EncryptString(plainUsername)).Returns(storedUsername);
        _mockAesEncryptionService.Setup(e => e.EncryptString(plainPassword)).Returns(storedPassword);

        // Act
        var valid = await service.AreCredentialsValidAsync(
            plainUsername,
            plainPassword);

        // Assert
        Assert.False(valid);
    }
}
