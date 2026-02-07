using System.Text.Json.Serialization;
using Refit;

namespace EmailSender.ElasticEmail;

public interface IElasticEmailApi
{
    [Post("/v4/emails/transactional")]
    Task SendTransactionalAsync([Body] ElasticEmailRequest request);
}

public record ElasticEmailRequest(
    [property: JsonPropertyName("Recipients")] ElasticEmailRecipients Recipients,
    [property: JsonPropertyName("Content")] ElasticEmailContent Content
);

public record ElasticEmailRecipients(
    [property: JsonPropertyName("To")] List<string> To
);

public record ElasticEmailContent(
    [property: JsonPropertyName("From")] string From,
    [property: JsonPropertyName("Subject")] string Subject,
    [property: JsonPropertyName("Body")] List<ElasticEmailBody> Body
);

public record ElasticEmailBody(
    [property: JsonPropertyName("ContentType")] string ContentType,
    [property: JsonPropertyName("Content")] string Content
);