using System.Net;

namespace Challenge.Common.Core.Response.Models
{
    /// <summary>
    /// Response indicating a resource conflict (409)
    /// </summary>
    /// <param name="errors">Dictionary of field names and their conflict messages</param>
    public class ConflictResponse(Dictionary<string, List<string>>? errors = null)
        : Response(statusCode: HttpStatusCode.Conflict, success: false)
    {
        private readonly Dictionary<string, List<string>> _errors = errors ?? [];

        /// <summary>
        /// Gets the conflict errors dictionary
        /// </summary>
        public Dictionary<string, List<string>> Errors => _errors;
    }
}
