using Challenge.Common.Data.Mongo.Entities;

namespace Challenge.Microservices.RiderApi.Infra.Data.Entities
{
    /// <summary>
    /// Represents a delivery rider in the system
    /// </summary>
    public class Rider : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier (business key)
        /// </summary>
        public required string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the full name of the rider
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the CNPJ (Brazilian tax identification number) - must be unique
        /// </summary>
        public required string Cnpj { get; set; }

        /// <summary>
        /// Gets or sets the birth date of the rider
        /// </summary>
        public required string Birthdate { get; set; }

        /// <summary>
        /// Gets or sets the CNH (Brazilian driver's license) number - must be unique
        /// </summary>
        public required string CnhNumber { get; set; }

        /// <summary>
        /// Gets or sets the CNH type/category (A, B, AB, etc.)
        /// </summary>
        public required string CnhType { get; set; }

        /// <summary>
        /// Gets or sets the URL of the uploaded CNH image (optional)
        /// </summary>
        public string? CnhImageUrl { get; set; }

        /// <summary>
        /// Gets or sets whether the rider is currently active in the system
        /// </summary>
        public bool Active { get; set; } = true;
    }
}
