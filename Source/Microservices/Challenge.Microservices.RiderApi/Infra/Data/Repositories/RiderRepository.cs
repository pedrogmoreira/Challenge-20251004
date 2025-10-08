using Challenge.Common.Data.Mongo.Repositories;
using Challenge.Microservices.RiderApi.Infra.Data.Entities;
using MongoDB.Driver;

namespace Challenge.Microservices.RiderApi.Infra.Data.Repositories
{
    /// <summary>
    /// Repository implementation for Rider entity
    /// </summary>
    /// <param name="database">MongoDB database instance</param>
    public class RiderRepository(IMongoDatabase database) : MongoRepository<Rider>(database, "Riders"), IRiderRepository
    {
        public async Task<Rider?> GetByIdentifierAsync(string identifier)
        {
            var filter = Builders<Rider>.Filter.Eq(r => r.Identifier, identifier);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> CnpjExistsAsync(string cnpj)
        {
            var filter = Builders<Rider>.Filter.Eq(r => r.Cnpj, cnpj);
            return await _collection.Find(filter).AnyAsync();
        }

        public async Task<bool> CnhNumberExistsAsync(string cnhNumber)
        {
            var filter = Builders<Rider>.Filter.Eq(r => r.CnhNumber, cnhNumber);
            return await _collection.Find(filter).AnyAsync();
        }

        public async Task<bool> IdentifierExistsAsync(string identifier)
        {
            var filter = Builders<Rider>.Filter.Eq(r => r.Identifier, identifier);
            return await _collection.Find(filter).AnyAsync();
        }
    }
}
