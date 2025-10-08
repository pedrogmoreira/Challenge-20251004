namespace Challenge.Common.Data.Models
{
    /// <summary>
    /// Represents a paginated result set
    /// </summary>
    /// <typeparam name="T">The type of entities in the result</typeparam>
    /// <param name="results">The entities for the current page</param>
    /// <param name="totalCount">Total number of entities across all pages</param>
    /// <param name="page">Current page number</param>
    /// <param name="pageSize">Number of items per page</param>
    public class PagedResult<T>(IEnumerable<T> results, int totalCount, int page, int pageSize) where T : class
    {
        /// <summary>
        /// Gets or sets the entities for the current page
        /// </summary>
        public IEnumerable<T> Results { get; set; } = results;

        /// <summary>
        /// Gets or sets the total number of entities across all pages
        /// </summary>
        public int TotalCount { get; set; } = totalCount;

        /// <summary>
        /// Gets or sets the current page number
        /// </summary>
        public int Page { get; set; } = page;

        /// <summary>
        /// Gets or sets the number of items per page
        /// </summary>
        public int PageSize { get; set; } = pageSize;

        /// <summary>
        /// Gets the total number of pages
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        /// <summary>
        /// Gets whether there is a previous page
        /// </summary>
        public bool HasPreviousPage => Page > 1;

        /// <summary>
        /// Gets whether there is a next page
        /// </summary>
        public bool HasNextPage => Page < TotalPages;
    }

}
