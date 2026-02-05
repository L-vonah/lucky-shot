using System.Net.Http.Headers;
using LuckyShot.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace EmailSender;

public static class MailerSendExtensions
{
    private const string BaseUrl = "https://api.mailersend.com";

    public static void AddMailerSendEmail(this IServiceCollection services, IConfiguration configuration)
    {
        var apiKey = configuration["Email:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Email:ApiKey must be configured.");
        }

        services.AddRefitClient<IMailerSendApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(BaseUrl);
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", apiKey);
            });

        services.AddScoped<IEmailSender, MailerSendEmailSender>();
    }
}