namespace Challenge.Microservices.SubscriptionApi.Infra.Services.Response
{
    /// <summary>
    /// DTO for rider information
    /// </summary>
    public class RiderResponse
    {
        /// <summary>
        /// The unique identifier (business key)
        /// </summary>
        public string? Identifier { get; set; }

        /// <summary>
        /// The full name of the rider
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The CNPJ (Brazilian tax identification number)
        /// </summary>
        public string? Cnpj { get; set; }

        /// <summary>
        /// The CNH type/category (A, B, AB, etc.)
        /// </summary>
        public string? CnhType { get; set; }

        /// <summary>
        /// The rider is currently active in the system
        /// </summary>
        public bool Active { get; set; }
    }
}