namespace Challenge.Microservices.SubscriptionApi.Infra.Messaging.Responses
{
    /// <summary>
    /// Response message indicating if motorbike has active rentals
    /// </summary>
    public class CheckActiveRentalsResponse
    {
        /// <summary>
        /// Indicates whether the rider has any active rental agreements in the system
        /// </summary>
        public bool HasActiveRentals { get; set; }

        /// <summary>
        /// The identifier of the motorbike to check for rental availability
        /// </summary>
        public string? MotorbikeIdentifier { get; set; }
    }
}