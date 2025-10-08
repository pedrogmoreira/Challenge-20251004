using System.Text.Json.Serialization;

namespace Challenge.Microservices.SubscriptionApi.Infra.Services.Response
{
    /// <summary>
    /// Response for cost calculation
    /// </summary>
    public class SubscriptionCostResponse
    {
        /// <summary>
        /// Gets or sets the total cost of the rental including all fees and penalties
        /// </summary>
        [JsonPropertyName("valor_total")]
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Gets or sets the number of days the motorbike was actually used
        /// </summary>
        [JsonPropertyName("dias_utilizados")]
        public int DaysUsed { get; set; }

        /// <summary>
        /// Gets or sets the total cost of daily rates for the rental period
        /// </summary>
        [JsonPropertyName("valor_diarias")]
        public decimal DailiesTotal { get; set; }

        /// <summary>
        /// Gets or sets the penalty fee charged for early return (20% or 40% based on plan)
        /// </summary>
        [JsonPropertyName("multa")]
        public decimal Penalty { get; set; }

        /// <summary>
        /// Gets or sets the number of days beyond the expected return date
        /// </summary>
        [JsonPropertyName("dias_adicionais")]
        public int AdditionalDays { get; set; }

        /// <summary>
        /// Gets or sets the additional cost charged for late return (R$50.00 per day)
        /// </summary>
        [JsonPropertyName("valor_adicional")]
        public decimal AdditionalCost { get; set; }
    }
}