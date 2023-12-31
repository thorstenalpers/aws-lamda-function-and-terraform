﻿namespace AuthenticationService.Backend.Repositories;

using Amazon.DynamoDBv2.DataModel;
using AuthenticationService.Backend.Exceptions;
using AuthenticationService.Backend.Models;
using System.Diagnostics.CodeAnalysis;

public interface ICredentialReadRepository
{
    Task<Credential> GetCredentialsAsync(string username);
}

public interface ICredentialWriteRepository
{
    Task SaveCredentialsAsync(Credential credential);
}

[ExcludeFromCodeCoverage]
public class CredentialRepository : ICredentialReadRepository, ICredentialWriteRepository
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

    public async Task SaveCredentialsAsync(Credential credential)
    {
        await _dynamoDBContext.SaveAsync(credential);
    }
}
