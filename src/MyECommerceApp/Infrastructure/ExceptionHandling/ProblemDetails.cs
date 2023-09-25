using System.Text.Json.Serialization;

namespace MyECommerceApp.Infrastructure.ExceptionHandling
{
    public class ProblemDetails
    {
        [JsonPropertyName("Type")]
        public string? Type { get; set; }

        [JsonPropertyName("Title")]
        public string? Title { get; set; }

        [JsonPropertyName("Status")]
        public int? Status { get; set; }

        [JsonPropertyName("Detail")]
        public string? Detail { get; set; }
        [JsonPropertyName("Instance")]
        public string Instance { get; set; }
        [JsonPropertyName("Errors")]
        public IDictionary<string, string[]> Errors { get; set; }
    }
}
