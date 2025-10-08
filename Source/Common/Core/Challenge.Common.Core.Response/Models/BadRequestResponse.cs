using System.Net;

namespace Challenge.Common.Core.Response.Models
{
    /// <summary>
    /// Response indicating a bad request with validation errors (400)
    /// </summary>
    /// <param name="errors">Dictionary of field names and their validation error messages</param>
    public class BadRequestResponse(Dictionary<string, List<string>>? errors = null)
        : Response(statusCode: HttpStatusCode.BadRequest, success: false)
    {
        private readonly Dictionary<string, List<string>> _errors = errors ?? [];

        /// <summary>
        /// Gets the validation errors dictionary
        /// </summary>
        public Dictionary<string, List<string>> Errors => _errors;
    }
}
