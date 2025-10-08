using Challenge.Common.Data.Mongo.Repositories;
using Challenge.Microservices.SubscriptionApi.Infra.Data.Entities;
using MongoDB.Driver;

namespace Challenge.Microservices.SubscriptionApi.Infra.Data.Repositories
{
    /// <summary>
    /// Repository implementation for Subscription
    /// </summary>
    public class SubscriptionRepository(IMongoDatabase database) : MongoRepository<Subscription>(database, "Subscriptions"), ISubscriptionRepository
    {
        public async Task<Subscription> GetByIdentifierAsync(string identifier)
        {
            var filter = Builders<Subscription>.Filter.Eq(s => s.Identifier, identifier);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Subscription>> GetByMotorbikeIdentifierAsync(string motorbikeIdentifier)
        {
            var filter = Builders<Subscription>.Filter.Eq(s => s.MotorbikeIdentifier, motorbikeIdentifier);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Subscription>> GetByRiderIdentifierAsync(string riderIdentifier)
        {
            var filter = Builders<Subscription>.Filter.Eq(s => s.RiderIdentifier, riderIdentifier);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<bool> HasActiveSubscriptionAsync(string motorbikeIdentifier)
        {
            var filter = Builders<Subscription>.Filter.And(
                Builders<Subscription>.Filter.Eq(s => s.MotorbikeIdentifier, motorbikeIdentifier),
                Builders<Subscription>.Filter.Eq(s => s.Status, SubscriptionStatus.Active)
            );

            return await _collection.Find(filter).AnyAsync();
        }
    }
}