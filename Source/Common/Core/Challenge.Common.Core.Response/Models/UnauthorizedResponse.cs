using System.Net;

namespace Challenge.Common.Core.Response.Models
{
    /// <summary>
    /// Response indicating authentication or authorization failure (401)
    /// </summary>
    /// <param name="errors">Optional dictionary of error messages</param>
    public class UnauthorizedResponse(Dictionary<string, List<string>>? errors = null)
        : Response(statusCode: HttpStatusCode.Unauthorized, success: false)
    {
        private readonly Dictionary<string, List<string>> _errors = errors ?? [];

        /// <summary>
        /// Gets the authentication/authorization errors dictionary
        /// </summary>
        public Dictionary<string, List<string>> Errors => _errors;
    }
}
