using Challenge.Common.Core.Cqrs.Interfaces;
using System.Text.Json.Serialization;

namespace Challenge.Microservices.SubscriptionApi.Commands
{
    /// <summary>
    /// Command to inform return date and calculate total cost
    /// </summary>
    public class InformReturnDateCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the unique identifier of the subscription (internal use only, not serialized)
        /// </summary>
        [JsonIgnore]
        public string? SubscriptionIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the actual return date when the motorbike will be or was returned
        /// </summary>
        [JsonPropertyName("data_devolucao")]
        public DateTime ReturnDate { get; set; }
    }
}