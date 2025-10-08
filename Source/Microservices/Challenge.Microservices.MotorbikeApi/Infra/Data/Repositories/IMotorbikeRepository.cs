using Challenge.Common.Data.Mongo.Interfaces;
using Challenge.Microservices.MotorbikeApi.Infra.Data.Entities;

namespace Challenge.Microservices.MotorbikeApi.Infra.Data.Repositories
{
    /// <summary>
    /// Repository interface for Motorbike entity operations
    /// </summary>
    public interface IMotorbikeRepository : IMongoRepository<Motorbike>
    {
        /// <summary>
        /// Gets a motorbike by its unique identifier
        /// </summary>
        Task<bool?> DisableByIdentifierAsync(string identifier);

        /// <summary>
        /// Gets all active motorbikes
        /// </summary>
        IEnumerable<Motorbike>? GetActive();

        /// <summary>
        /// Gets a motorbike by its unique identifier
        /// </summary>
        Task<Motorbike?> GetByIdentifierAsync(string identifier);

        /// <summary>
        /// Gets a motorbike by its license plate
        /// </summary>
        Task<Motorbike?> GetByLicensePlateAsync(string licensePlate);

        /// <summary>
        /// Checks if a motorbike has any active rental subscriptions
        /// </summary>
        Task<bool> HasActiveRentalsAsync(string identifier);

        /// <summary>
        /// Checks if a license plate is already registered
        /// </summary>
        Task<bool> LicensePlateExistsAsync(string licensePlate);

        /// <summary>
        /// Checks if an identifier is already in use
        /// </summary>
        Task<bool> IdentifierExistsAsync(string identifier);
    }
}