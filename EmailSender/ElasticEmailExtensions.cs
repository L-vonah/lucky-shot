using LuckyShot.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace EmailSender;

public static class ElasticEmailExtensions
{
    private const string BaseUrl = "https://api.elasticemail.com";

    public static void AddElasticEmail(this IServiceCollection services, IConfiguration configuration)
    {
        var apiKey = configuration["Email:Elastic:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Email:Elastic:ApiKey must be configured.");
        }

        services.AddRefitClient<IElasticEmailApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(BaseUrl);
                c.DefaultRequestHeaders.Add(name: "X-ElasticEmail-ApiKey", apiKey);
            });

        services.AddScoped<IEmailSender, ElasticEmailSender>();
    }
}