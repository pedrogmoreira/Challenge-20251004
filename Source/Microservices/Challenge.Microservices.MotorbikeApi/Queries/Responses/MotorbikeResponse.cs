namespace Challenge.Microservices.MotorbikeApi.Queries.Responses
{
    /// <summary>
    /// Response message with motorbike details (no id)
    /// </summary>
    public class MotorbikeResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier for the motorbike
        /// </summary>
        public required string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the manufacturing year of the motorbike
        /// </summary>
        public required int Year { get; set; }

        /// <summary>
        /// Gets or sets the model of the motorbike
        /// </summary>
        public required string Model { get; set; }

        /// <summary>
        /// Gets or sets the license plate - must be unique
        /// </summary>
        public required string LicensePlate { get; set; }

        /// <summary>
        /// Gets or sets whether the motorbike is currently available for rental
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the motorbike is currently active in the system
        /// </summary>
        public bool Active { get; set; }
    }
}
