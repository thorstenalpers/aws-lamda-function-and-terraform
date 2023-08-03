namespace AuthenticationService.Backend.Options;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class AuthenticationServiceOptions
{
    public const string SectionName = "Authentication";

    /// <summary> Base64 Encoded Salt, 16 Bytes </summary>
    public string Salt { get; set; }

    /// <summary>ApiKey to allow access to the api</summary>
    public string ApiKey { get; set; }
}
