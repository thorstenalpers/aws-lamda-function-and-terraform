namespace AuthenticationService.Exceptions;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class AuthenticatorException : Exception
{
    public AuthenticatorException()
    {
    }

    public AuthenticatorException(string message)
        : base(message)
    {
    }

    public AuthenticatorException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
