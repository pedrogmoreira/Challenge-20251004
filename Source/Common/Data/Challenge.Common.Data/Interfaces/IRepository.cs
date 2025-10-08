using Challenge.Common.Data.Models;

namespace Challenge.Common.Data.Interfaces
{
    /// <summary>
    /// Base repository interface for data access operations
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves all entities from the repository
        /// </summary>
        /// <returns>Collection of all entities</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity">The entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        Task<bool> UpdateAsync(T entity);

        /// <summary>
        /// Retrieves a paginated list of entities
        /// </summary>
        /// <param name="page">The page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paginated result containing entities and metadata</returns>
        Task<PagedResult<T>> GetPagedAsync(int page, int pageSize);
    }
}
