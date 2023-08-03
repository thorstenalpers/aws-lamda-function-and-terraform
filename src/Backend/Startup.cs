namespace AuthenticationService.Backend;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AuthenticationService.Backend.Options;
using AuthenticationService.Backend.Repositories;
using AuthenticationService.Backend.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
        services.Configure<AuthenticationServiceOptions>(Configuration.GetSection(AuthenticationServiceOptions.SectionName));
        services.AddAWSService<IAmazonDynamoDB>();
        services.AddScoped<IDynamoDBContext>(provider =>
        {
            var dynamoDbConfig = new DynamoDBContextConfig
            {
                TableNamePrefix = "Credentials",
                ConsistentRead = true
            };
            var dynamoDbClient = provider.GetRequiredService<IAmazonDynamoDB>();
            return new DynamoDBContext(dynamoDbClient, dynamoDbConfig);
        });

        services.AddScoped<ICredentialRepository, CredentialRepository>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddLogging(ConfigureLogging);
        services.AddAuthentication("ServiceAuthenticationScheme")
            .AddScheme<AuthenticationSchemeOptions, ServiceAuthenticationHandler>("ServiceAuthenticationScheme", null);

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ServiceToServicePolicy", policy =>
            {
                policy.AuthenticationSchemes.Add("ServiceAuthenticationScheme");
                policy.RequireAuthenticatedUser();
            });
        });

        services.AddSingleton<IAuthorizationHandler, ServiceToServiceAuthorizationHandler>();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    private void ConfigureLogging(ILoggingBuilder loggingBuilder)
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.AddLambdaLogger();
    }
}