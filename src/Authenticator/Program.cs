namespace Authenticator;

using Microsoft.AspNetCore.Builder;

public class Program
{
    public static void Main()
    {
        var app = CreateWebApplication();
        app.Run();
    }

    public static WebApplication CreateWebApplication()
    {
        var builder = WebApplication.CreateBuilder();

        var configuration = builder.Configuration;
        var services = builder.Services;

        var app = builder.Build();

        return app;
    }
}

