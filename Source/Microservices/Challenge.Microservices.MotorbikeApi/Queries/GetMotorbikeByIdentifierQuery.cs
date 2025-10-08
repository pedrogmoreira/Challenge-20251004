using Challenge.Common.Core.Cqrs.Interfaces;
using System.Text.Json.Serialization;

namespace Challenge.Microservices.MotorbikeApi.Queries
{
    /// <summary>
    /// Query parameters for retrieving motorbike(s)
    /// </summary>
    public class GetMotorbikeByIdentifierQuery : IQuery
    {
        /// <summary>
        /// Filter by unique identifier
        /// </summary>
        [JsonPropertyName("identificador")]
        public string? Identifier { get; set; }
    }
}
