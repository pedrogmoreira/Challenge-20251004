using Challenge.Common.Core.Cqrs.Interfaces;
using System.Text.Json.Serialization;

namespace Challenge.Microservices.RiderApi.Commands
{
    /// <summary>
    /// Command to upload CNH image
    /// </summary>
    public class UploadRiderCnhImageCommand : ICommand
    {
        /// <summary>
        /// The unique identifier of the rider
        /// </summary>
        [JsonIgnore]
        public string? Identifier { get; set; }

        /// <summary>
        /// Base64 encoded image with data URI prefix
        /// </summary>
        [JsonPropertyName("imagem_cnh")]
        public required string CnhImage { get; set; }
    }
}
