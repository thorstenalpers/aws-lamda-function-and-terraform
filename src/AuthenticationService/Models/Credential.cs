namespace AuthenticationService.Models;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class Credential
{
    public string Username { get; set; }
    public string Password { get; set; }
}
