using Challenge.Common.Core.Cqrs.Interfaces;
using System.Text.Json.Serialization;

namespace Challenge.Microservices.MotorbikeApi.Commands
{
    /// <summary>
    /// Command to update motorbike license plate
    /// </summary>
    public class UpdateMotorbikeLicensePlateCommand : ICommand
    {
        /// <summary>
        /// The motorbike identifier
        /// </summary>
        [JsonIgnore]
        public string? Identifier { get; set; }

        /// <summary>
        /// The new license plate
        /// </summary>
        [JsonPropertyName("placa")]
        public string? LicensePlate { get; set; }
    }
}