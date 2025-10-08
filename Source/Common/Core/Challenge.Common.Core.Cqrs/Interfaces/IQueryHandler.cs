using Challenge.Common.Core.Response.Models.Interfaces;

namespace Challenge.Common.Core.Cqrs.Interfaces
{
    /// <summary>
    /// Command handler interface for queries that return a response
    /// </summary>
    /// <typeparam name="TCommand">The query type to be handled</typeparam>
    public interface IQueryHandler<in TQuery> where TQuery : IQuery
    {
        /// <summary>
        /// Handles the query execution
        /// </summary>
        /// <param name="command">The query to execute</param>
        /// <returns>A response indicating the result of the operation</returns>
        Task<IResponse> Handle(TQuery query);
    }
}
