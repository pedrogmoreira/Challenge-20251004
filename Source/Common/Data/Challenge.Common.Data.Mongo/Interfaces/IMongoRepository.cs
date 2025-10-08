using Challenge.Common.Data.Interfaces;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace Challenge.Common.Data.Mongo.Interfaces
{
    /// <summary>
    /// MongoDB-specific repository interface with additional operations
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public interface IMongoRepository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves an entity by its unique identifier
        /// </summary>
        /// <param name="id">The entity ID as a string</param>
        /// <returns>The entity if found, null otherwise</returns>
        Task<T?> GetByIdAsync(string id);

        /// <summary>
        /// Deletes an entity by its unique identifier
        /// </summary>
        /// <param name="id">The entity ID as a string</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        Task<bool> DeleteByIdAsync(string id);

        /// <summary>
        /// Adds a new entity to the repository
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <returns>The ObjectId of the newly created entity</returns>
        Task<ObjectId> AddAsync(T entity);

        /// <summary>
        /// Checks if any entity matches the specified predicate
        /// </summary>
        /// <param name="predicate">Expression to filter entities</param>
        /// <returns>True if at least one entity matches, false otherwise</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
