namespace AuthenticationService.Options;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class AuthenticationServiceOptions
{
    public const string SectionName = "Authentication";

    /// <summary> Base64 Encoded Cipher Key, 32 Bytes </summary>
    public string CipherKey { get; set; }

    /// <summary> Base64 Encoded Cipher IV, 16 Bytes </summary>
    public string CipherIv { get; set; }

    /// <summary>ApiKey to allow access to the api</summary>
    public string ApiKey { get; set; }
}
