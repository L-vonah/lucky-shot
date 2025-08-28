using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace ApiFootball.Extensions;

public static class ApiFootballExtensions
{
    private static string HeaderKey => "X-Auth-Token";
    
    public static void AddApiFootballServices(this IServiceCollection services, IConfiguration configuration)
    {
        var baseUrl = configuration.GetValue<string>("ApiFootball:BaseUrl")!;
        var apiKeyHeader = configuration.GetValue<string>("ApiFootball:ApiKey")!;
        services.AddRefitClient<IApiFootball>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(baseUrl);
                c.DefaultRequestHeaders.Add(HeaderKey, apiKeyHeader);
            });
    }
}