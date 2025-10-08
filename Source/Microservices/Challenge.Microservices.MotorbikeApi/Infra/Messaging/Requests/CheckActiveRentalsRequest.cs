namespace Challenge.Microservices.MotorbikeApi.Infra.Messaging.Requests
{
    /// <summary>
    /// Request message to check if motorbike has active rentals
    /// </summary>
    public class CheckActiveRentalsRequest
    {
        /// <summary>
        /// The identifier of the motorbike to check for rental availability
        /// </summary>
        public string? MotorbikeIdentifier { get; set; }
    }
}
