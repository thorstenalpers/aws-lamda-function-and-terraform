namespace AuthenticationService.Tests.Services;

using AuthenticationService.Options;
using AuthenticationService.Security;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

[TestFixture]
[Category("UnitTests")]
public class AesEncryptionServiceTests
{
    private MockRepository _mockRepository;
    private Mock<IOptionsSnapshot<AuthenticationServiceOptions>> _mockOptionsSnapshot;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new MockRepository(MockBehavior.Strict);

        _mockOptionsSnapshot = _mockRepository.Create<IOptionsSnapshot<AuthenticationServiceOptions>>();
        _mockOptionsSnapshot.Setup(x => x.Value).Returns(
            new AuthenticationServiceOptions
            {
                CipherIv = "U2FsdGVkc3NzYWRzZmRhcw==",
                CipherKey = "VGhpcyBpcyBhIGJhc2U2NCBlbmNvZGluZyBzdHJpbmc="
            });
    }

    [TestCase]
    public void EncryptString_Success()
    {
        // Arrange
        var service = new AesEncryptionService(_mockOptionsSnapshot.Object);
        string plainText = "Thorsten";

        // Act
        var encryptedString = service.EncryptString(plainText);

        // Assert
        Assert.AreEqual("3DwlLS3hBF05eMWFa0T0fw==", encryptedString);
    }

    [Test]
    public void DecryptString_Success()
    {
        // Arrange
        var service = new AesEncryptionService(_mockOptionsSnapshot.Object);
        string encryptedString = "3DwlLS3hBF05eMWFa0T0fw==";

        // Act
        var decryptedString = service.DecryptString(encryptedString);

        // Assert
        Assert.AreEqual("Thorsten", decryptedString);
    }

    [TestCase]
    public void EncryptAndDecryptString_Success()
    {
        // Arrange
        var service = new AesEncryptionService(_mockOptionsSnapshot.Object);
        string plainText = "Thorsten";

        // Act
        var encryptedString = service.EncryptString(plainText);
        var decryptedString = service.DecryptString(encryptedString);

        // Assert
        Assert.AreEqual(plainText, decryptedString);
    }
}
