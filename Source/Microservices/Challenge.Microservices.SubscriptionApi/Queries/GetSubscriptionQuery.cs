using Challenge.Common.Core.Cqrs.Interfaces;
using System.Text.Json.Serialization;

namespace Challenge.Microservices.SubscriptionApi.Queries
{
    /// <summary>
    /// Query parameters for retrieving subscription
    /// </summary>
    public class GetSubscriptionQuery : IQuery
    {
        /// <summary>
        /// Filter by unique identifier
        /// </summary>
        [JsonPropertyName("identificador")]
        public string? Identifier { get; set; }
    }
}
