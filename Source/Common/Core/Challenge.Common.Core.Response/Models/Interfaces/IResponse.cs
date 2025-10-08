using System.Net;

namespace Challenge.Common.Core.Response.Models.Interfaces
{
    /// <summary>
    /// Base interface for all response types
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Gets the HTTP status code for this response
        /// </summary>
        /// <returns>The HTTP status code</returns>
        HttpStatusCode GetStatusCode();
    }
}
