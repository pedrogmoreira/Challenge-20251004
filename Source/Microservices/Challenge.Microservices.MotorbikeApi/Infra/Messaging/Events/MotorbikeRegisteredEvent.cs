namespace Challenge.Microservices.MotorbikeApi.Infra.Messaging.Events
{
    /// <summary>
    /// Event published when a new motorbike is registered in the system
    /// Contains the motorbike information for messaging consumers
    /// </summary>
    public class MotorbikeRegisteredEvent
    {
        /// <summary>
        /// Gets or sets the unique identifier of the motorbike
        /// </summary>
        public string? MotorbkeIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the manufacturing year of the motorbike
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the model name of the motorbike
        /// </summary>
        public string? Model { get; set; }

        /// <summary>
        /// Gets or sets the license plate number - must be unique
        /// </summary>
        public string? LicensePlate { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp when the event was created
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}