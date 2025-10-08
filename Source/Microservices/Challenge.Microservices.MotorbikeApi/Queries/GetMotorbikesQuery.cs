using Challenge.Common.Core.Cqrs.Interfaces;
using System.Text.Json.Serialization;

namespace Challenge.Microservices.MotorbikeApi.Queries
{
    /// <summary>
    /// Query parameters for retrieving motorbike(s)
    /// </summary>
    public class GetMotorbikesQuery : IQuery
    {
        /// <summary>
        /// Filter by license plate
        /// </summary>
        [JsonPropertyName("placa")]
        public string? LicensePlate { get; set; }
    }
}
