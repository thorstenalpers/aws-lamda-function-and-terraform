namespace AuthenticationService.Tests.Services;

using AuthenticationService.Backend.Repositories;
using AuthenticationService.Backend.Security;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using AuthenticationService.Backend.Services;
using AuthenticationService.Backend.Models;

[TestFixture]
[Category("UnitTests")]
public class AuthenticationServiceTests
{
    private MockRepository _mockRepository;
    private Mock<ICredentialRepository> _mockCredentialRepository;
    private Mock<IPasswordHasherService> _mockPasswordHasherService;
    private Mock<ILogger<AuthenticationService>> _mockLogger;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new MockRepository(MockBehavior.Strict);

        _mockCredentialRepository = _mockRepository.Create<ICredentialRepository>();
        _mockPasswordHasherService = _mockRepository.Create<IPasswordHasherService>();
        _mockLogger = new Mock<ILogger<AuthenticationService>>();
    }

    private AuthenticationService CreateService()
    {
        return new AuthenticationService(
            _mockLogger.Object,
            _mockCredentialRepository.Object,
            _mockPasswordHasherService.Object);
    }

    [Test]
    public async Task AreCredentialsValidAsync_Success()
    {
        // Arrange
        var service = CreateService();
        var plainCreds = new Credential
        {
            Username = "Username",
            Password = "Password"
        };
        var hashedCreds = new Credential
        {
            Username = "Username",
            Password = "$2a$12$hG5lf/9xPbovhz8kATDgd.IpixYS9k3TuKLckJmE4CAMRlMNTmPxu"
        };
        _mockCredentialRepository.Setup(x => x.GetCredentialsAsync(hashedCreds.Username)).ReturnsAsync(hashedCreds);
        _mockPasswordHasherService.Setup(x => x.VerifyPassword(hashedCreds.Password, plainCreds.Password)).Returns(true);

        // Act
        var valid = await service.AreCredentialsValidAsync(
            plainCreds.Username,
            plainCreds.Password);

        // Assert
        Assert.True(valid);
    }

    [TestCase("plainUsername", "", "hashedUsername", "hashedPassword")]
    [TestCase("", "plainPassword", "encryptedUsername", "hashedPassword")]
    [TestCase("", "", "encryptedUsername", "hashedPassword")]
    [TestCase("plainUsername", "plainPassword", "encryptedUsername", "WrongPassword")]
    public async Task AreCredentialsValidAsync_Fail(string plainUsername, string plainPassword, string storedUsername, string storedPassword)
    {
        // Arrange
        var service = CreateService();

        _mockCredentialRepository.Setup(x => x.GetCredentialsAsync(storedUsername)).ReturnsAsync(
            new Credential
            {
                Username = storedUsername,
                Password = storedPassword
            });

        // Act
        var valid = await service.AreCredentialsValidAsync(
            plainUsername,
            plainPassword);

        // Assert
        Assert.False(valid);
    }
}
