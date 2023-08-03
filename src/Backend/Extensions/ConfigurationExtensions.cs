namespace AuthenticationService.Backend.Extensions;

using Amazon;
using Amazon.AppConfig;
using Amazon.AppConfig.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

public static class ConfigurationExtensions
{
    public static IConfigurationBuilder AddAmazonAppConfig(this IConfigurationBuilder builder, string applicationName)
    {
        string regionFromEnv = System.Environment.GetEnvironmentVariable("AWS_REGION");

        var appConfigConfig = new AmazonAppConfigConfig
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(regionFromEnv),
        };
        using var appConfigClient = new AmazonAppConfigClient(appConfigConfig);
        var request = new GetConfigurationRequest
        {
            Application = applicationName,
            Environment = regionFromEnv
        };

#pragma warning disable CS0618 // Type or member is obsolete (not implemented in C# client)
        var response = appConfigClient.GetConfigurationAsync(request).Result;
#pragma warning restore CS0618 // Type or member is obsolete

        // Read the MemoryStream and parse the JSON content
        using (var reader = new StreamReader(response.Content))
        {
            var jsonContent = reader.ReadToEnd();
            var configValues = new List<KeyValuePair<string, string>>();

            // Iterate over the key-value pairs in the JSON content and add the
            foreach (var kvp in JObject.Parse(jsonContent))
            {
                var key = kvp.Key.ToString();
                var value = kvp.Value.ToString();
                configValues.Add(new KeyValuePair<string, string>(key, value));
            }
            builder.AddInMemoryCollection(configValues);
        }
        return builder;
    }
}
