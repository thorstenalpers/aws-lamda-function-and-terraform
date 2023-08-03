namespace AuthenticationService.Repositories;

using Amazon.DynamoDBv2.DataModel;
using AuthenticationService.Exceptions;
using AuthenticationService.Models;
using System.Diagnostics.CodeAnalysis;

public interface ICredentialRepository
{
    Task<Credential> GetCredentialsAsync(string username);
}

[ExcludeFromCodeCoverage]
public class CredentialRepository : ICredentialRepository
{
    private readonly IDynamoDBContext _dynamoDBContext;

    public CredentialRepository(IDynamoDBContext dynamoDBContext)
    {
        _dynamoDBContext = dynamoDBContext;
    }

    public async Task<Credential> GetCredentialsAsync(string username)
    {
        var credential = await _dynamoDBContext.LoadAsync<Credential>(username);
        if (credential?.Password == null)
            throw new AuthenticatorException($"No record found for {username}");

        return credential;
    }
}
