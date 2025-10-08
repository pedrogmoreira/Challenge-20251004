using Challenge.Common.Data.Mongo.Interfaces;
using Challenge.Microservices.SubscriptionApi.Infra.Data.Entities;

namespace Challenge.Microservices.SubscriptionApi.Infra.Data.Repositories
{
    /// <summary>
    /// Repository interface for Subscription operations
    /// </summary>
    public interface ISubscriptionRepository : IMongoRepository<Subscription>
    {
        /// <summary>
        /// Gets a subscription
        /// </summary>
        Task<Subscription> GetByIdentifierAsync(string identifier);

        /// <summary>
        /// Gets all subscriptions for a specific motorbike
        /// </summary>
        Task<IEnumerable<Subscription>> GetByMotorbikeIdentifierAsync(string motorbikeIdentifier);

        /// <summary>
        /// Gets all subscriptions for a specific rider
        /// </summary>
        Task<IEnumerable<Subscription>> GetByRiderIdentifierAsync(string riderIdentifier);

        /// <summary>
        /// Checks if a motorbike has any active subscriptions
        /// </summary>
        Task<bool> HasActiveSubscriptionAsync(string motorbikeIdentifier);
    }
}