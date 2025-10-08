using Challenge.Common.Core.Cqrs.Interfaces;
using System.Text.Json.Serialization;

namespace Challenge.Microservices.SubscriptionApi.Commands
{
    /// <summary>
    /// Command to create a new rental subscription
    /// </summary>
    public class CreateSubscriptionCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the unique identifier of the rental subscription
        /// </summary>
        [JsonPropertyName("identificador")]
        public string? Identifier { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the rider creating the rental
        /// </summary>
        [JsonPropertyName("entregador_id")]
        public string? RiderIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the motorbike to be rented
        /// </summary>
        [JsonPropertyName("moto_id")]
        public string? MotorbikeIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the rental start date (must be the first day after creation)
        /// </summary>
        [JsonPropertyName("data_inicio")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the rental end date based on the selected plan
        /// </summary>
        [JsonPropertyName("data_termino")]
        public DateTime ExpectedEndDate { get; set; }

        /// <summary>
        /// Gets or sets the predicted end date for calculating early or late return penalties
        /// </summary>
        [JsonPropertyName("data_previsao_termino")]
        public DateTime PredictedEndDate { get; set; }

        /// <summary>
        /// Gets or sets the rental plan duration in days (7, 15, 30, 45, or 50 days)
        /// </summary>
        [JsonPropertyName("plano")]
        public int PlanDays { get; set; }
    }
}