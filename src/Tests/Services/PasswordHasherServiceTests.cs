namespace AuthenticationService.Tests.Services;

using AuthenticationService.Backend.Options;
using AuthenticationService.Backend.Security;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

[TestFixture]
[Category("UnitTests")]
public class PasswordHasherServiceTests
{

    [TestCase]
    public void HashPassword_Success()
    {
        // Arrange
        var service = new PasswordHasherService();
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
        var service = new PasswordHasherService();
        string plainPassword = "Password";
        string hashedPassword = "$2a$12$hG5lf/9xPbovhz8kATDgd.IpixYS9k3TuKLckJmE4CAMRlMNTmPxu";

        var isValid = service.VerifyPassword(hashedPassword, plainPassword);

        // Assert
        Assert.True(isValid);
    }
}
