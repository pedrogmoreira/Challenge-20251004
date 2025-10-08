using Challenge.Common.Core.Cqrs.Interfaces;
using System.Text.Json.Serialization;

namespace Challenge.Microservices.MotorbikeApi.Commands
{
    /// <summary>
    /// Command to register a new motorbike
    /// </summary>
    public class RegisterMotorbikeCommand : ICommand
    {
        /// <summary>
        /// Unique identifier for the motorbike
        /// </summary>
        [JsonPropertyName("identificador")]
        public string? Identifier { get; set; }

        /// <summary>
        /// Manufacturing year
        /// </summary>
        [JsonPropertyName("ano")]
        public int Year { get; set; }

        /// <summary>
        /// Motorbike model
        /// </summary>
        [JsonPropertyName("modelo")]
        public string? Model { get; set; }

        /// <summary>
        /// License plate (must be unique)
        /// </summary>
        [JsonPropertyName("placa")]
        public string? LicensePlate { get; set; }
    }
}