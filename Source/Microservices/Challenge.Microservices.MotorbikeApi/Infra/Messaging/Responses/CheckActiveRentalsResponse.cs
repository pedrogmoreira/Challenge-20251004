namespace Challenge.Microservices.MotorbikeApi.Infra.Messaging.Responses
{
    /// <summary>
    /// Response message indicating if motorbike has active rentals
    /// </summary>
    public class CheckActiveRentalsResponse
    {
        public bool HasActiveRentals { get; set; }
        public string? MotorbikeIdentifier { get; set; }
    }
}
