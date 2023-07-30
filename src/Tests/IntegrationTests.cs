namespace Authenticator.Tests;

using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

[TestFixture]
[Category("IntegrationTests")]
public class IntegrationTests
{
    private IServiceProvider _serviceProvider;

    [Test]
    public void Test_1()
    {


        Assert.Pass();
    }

    [SetUp]
    public void Setup()
    {
        var host = Program.CreateWebApplication();
        var scope = host.Services.CreateScope();
        _serviceProvider = scope.ServiceProvider;
    }
}
