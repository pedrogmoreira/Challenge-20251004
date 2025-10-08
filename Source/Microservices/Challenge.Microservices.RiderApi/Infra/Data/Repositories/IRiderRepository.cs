using Challenge.Common.Data.Mongo.Interfaces;
using Challenge.Microservices.RiderApi.Infra.Data.Entities;

namespace Challenge.Microservices.RiderApi.Infra.Data.Repositories
{
    /// <summary>
    /// Repository interface for Rider entity operations
    /// </summary>
    public interface IRiderRepository : IMongoRepository<Rider>
    {
        /// <summary>
        /// Gets a rider by identifier
        /// </summary>
        Task<Rider?> GetByIdentifierAsync(string identifier);

        /// <summary>
        /// Checks if CNPJ already exists
        /// </summary>
        Task<bool> CnpjExistsAsync(string cnpj);

        /// <summary>
        /// Checks if CNH number already exists
        /// </summary>
        Task<bool> CnhNumberExistsAsync(string cnhNumber);

        /// <summary>
        /// Checks if identifier already exists
        /// </summary>
        Task<bool> IdentifierExistsAsync(string identifier);
    }
}