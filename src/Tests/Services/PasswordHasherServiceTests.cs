namespace AuthenticationService.Tests.Services;

using AuthenticationService.Options;
using AuthenticationService.Security;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

[TestFixture]
[Category("UnitTests")]
public class PasswordHasherServiceTests
{
    private MockRepository _mockRepository;
    private Mock<IOptionsSnapshot<AuthenticationServiceOptions>> _mockAuthOptions;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new MockRepository(MockBehavior.Strict);

        _mockAuthOptions = _mockRepository.Create<IOptionsSnapshot<AuthenticationServiceOptions>>();
        _mockAuthOptions.Setup(x => x.Value).Returns(
            new AuthenticationServiceOptions
            {
                Salt = "$2a$12$hG5lf/9xPbovhz8kATDgd."
            });
    }

    [TestCase]
    public void HashPassword_Success()
    {
        // Arrange
        var service = new PasswordHasherService(_mockAuthOptions.Object);
        string plainPassword = "Password";

        // Act
        var hashedPassword = service.HashPassword(plainPassword);

        // Assert
        Assert.AreEqual("$2a$12$hG5lf/9xPbovhz8kATDgd.IpixYS9k3TuKLckJmE4CAMRlMNTmPxu", hashedPassword);
    }

    [TestCase]
    public void VerifyPassword_Success()
    {
        // Arrange
        var service = new PasswordHasherService(_mockAuthOptions.Object);
        string plainPassword = "Password";
        string hashedPassword = "$2a$12$hG5lf/9xPbovhz8kATDgd.IpixYS9k3TuKLckJmE4CAMRlMNTmPxu";

        var isValid = service.VerifyPassword(hashedPassword, plainPassword);

        // Assert
        Assert.True(isValid);
    }
}
