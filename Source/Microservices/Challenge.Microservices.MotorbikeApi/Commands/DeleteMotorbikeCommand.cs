using Challenge.Common.Core.Cqrs.Interfaces;
using System.Text.Json.Serialization;

namespace Challenge.Microservices.MotorbikeApi.Commands
{
    /// <summary>
    /// Command to delete a motorbike
    /// </summary>
    public class DeleteMotorbikeCommand : ICommand
    {
        /// <summary>
        /// The motorbike identifier to delete
        /// </summary>
        [JsonIgnore]
        public required string Identifier { get; set; }
    }
}