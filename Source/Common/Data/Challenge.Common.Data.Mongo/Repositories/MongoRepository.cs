using MongoDB.Driver;
using Challenge.Common.Data.Models;
using MongoDB.Bson;
using Challenge.Common.Data.Mongo.Entities;
using Challenge.Common.Data.Mongo.Interfaces;
using System.Linq.Expressions;

namespace Challenge.Common.Data.Mongo.Repositories
{
    /// <summary>
    /// Abstract base repository implementation for MongoDB operations
    /// </summary>
    /// <typeparam name="T">The entity type that inherits from BaseEntity</typeparam>
    /// <param name="database">The MongoDB database instance</param>
    /// <param name="collectionName">The name of the collection</param>
    public abstract class MongoRepository<T>(IMongoDatabase database, string collectionName) : IMongoRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// The MongoDB collection for this repository
        /// </summary>
        protected readonly IMongoCollection<T> _collection = database.GetCollection<T>(collectionName);

        /// <summary>
        /// Retrieves all entities from the collection
        /// </summary>
        /// <returns>Collection of all entities</returns>
        public IEnumerable<T> GetAll()
        {
            return _collection.AsQueryable();
        }

        /// <summary>
        /// Retrieves an entity by its ObjectId
        /// </summary>
        /// <param name="id">The entity ID as a string</param>
        /// <returns>The entity if found, null otherwise</returns>
        public async Task<T?> GetByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<T>.Filter.Eq(e => e.Id, objectId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Adds a new entity to the collection and returns its generated ID
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <returns>The ObjectId of the newly created entity</returns>
        public async Task<ObjectId> AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);

            var idProperty = typeof(T).GetProperty("Id");

            if (idProperty != null && idProperty.GetValue(entity) is ObjectId objectId)
            {
                return objectId;
            }

            return ObjectId.Empty;
        }

        /// <summary>
        /// Updates an existing entity in the collection
        /// </summary>
        /// <param name="entity">The entity with updated values</param>
        /// <returns>True if update was successful, false otherwise</returns>
        public async Task<bool> UpdateAsync(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
            {
                return false;
            }

            var idValue = idProperty.GetValue(entity);

            if (idValue is not ObjectId objectId)
            {
                return false;
            }

            var updateDefinitions = new List<UpdateDefinition<T>>();
            var updateBuilder = Builders<T>.Update;

            foreach (var property in typeof(T).GetProperties())
            {
                if (property.Name == "Id")
                {
                    continue;
                }

                var value = property.GetValue(entity);

                if (value != null && !(value is string strValue && string.IsNullOrEmpty(strValue)))
                {
                    updateDefinitions.Add(updateBuilder.Set(property.Name, value));
                }
            }

            if (updateDefinitions.Count == 0)
            {
                return false;
            }

            var updateDefinition = updateBuilder.Combine(updateDefinitions);

            var result = await _collection.UpdateOneAsync(
                Builders<T>.Filter.Eq("_id", objectId),
                updateDefinition
            );

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        /// <summary>
        /// Deletes an entity by its ObjectId
        /// </summary>
        /// <param name="id">The entity ID as a string</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<T>.Filter.Eq(e => e.Id, objectId);

            var result = await _collection.DeleteOneAsync(filter);

            return result.DeletedCount > 0;
        }

        /// <summary>
        /// Retrieves a paginated list of entities with optional filtering
        /// </summary>
        /// <param name="page">The page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="filters">Optional MongoDB filter definition</param>
        /// <returns>Paginated result containing entities and metadata</returns>
        public async Task<PagedResult<T>> GetPagedAsync(int page, int pageSize, FilterDefinition<T> filters)
        {
            var totalRecords = await _collection.CountDocumentsAsync(filters ?? FilterDefinition<T>.Empty);
            var results = await _collection.Find(filters ?? FilterDefinition<T>.Empty)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return new PagedResult<T>(results, (int)totalRecords, page, pageSize);
        }

        /// <summary>
        /// Retrieves a paginated list of all entities
        /// </summary>
        /// <param name="page">The page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paginated result containing entities and metadata</returns>
        public async Task<PagedResult<T>> GetPagedAsync(int page, int pageSize)
        {
            return await GetPagedAsync(page, pageSize, FilterDefinition<T>.Empty);
        }

        /// <summary>
        /// Checks if any entity matches the specified predicate
        /// </summary>
        /// <param name="predicate">Expression to filter entities</param>
        /// <returns>True if at least one entity matches, false otherwise</returns>
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).AnyAsync();
        }
    }
}
