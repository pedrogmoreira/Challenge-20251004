using Challenge.Common.Core.Cqrs.Interfaces;
using System.Text.Json.Serialization;

namespace Challenge.Microservices.RiderApi.Commands
{
    /// <summary>
    /// Command to register a new rider in the system
    /// </summary>
    public class RegisterRiderCommand : ICommand
    {
        /// <summary>
        /// Unique identifier for the rider
        /// </summary>
        [JsonPropertyName("identificador")]
        public string? Identifier { get; set; }

        /// <summary>
        /// The full name of the rider
        /// </summary>
        [JsonPropertyName("nome")]
        public string? Name { get; set; }

        /// <summary>
        /// The CNPJ (Brazilian tax identification number) - must be unique
        /// </summary>
        [JsonPropertyName("cnpj")]
        public string? Cnpj { get; set; }

        /// <summary>
        /// The birth date of the rider - used for age verification
        /// </summary>
        [JsonPropertyName("data_nascimento")]
        public DateTime Birthdate { get; set; }

        /// <summary>
        /// The CNH (Brazilian driver's license) number - must be unique
        /// </summary>
        [JsonPropertyName("numero_cnh")]
        public string? CnhNumber { get; set; }

        /// <summary>
        /// The type/category of the driver's license (A, B, AB, etc.)
        /// </summary>
        [JsonPropertyName("tipo_cnh")]
        public string? CnhType { get; set; }

        /// <summary>
        /// The URL of the uploaded CNH image (optional during registration)
        /// </summary>
        [JsonPropertyName("imagem_cnh")]
        public string? CnhImageUrl { get; set; }
    }
}
