using System.Net;

namespace Challenge.Common.Core.Response.Models
{
    /// <summary>
    /// Response indicating successful resource creation (201)
    /// </summary>
    /// <typeparam name="T">The type of the created resource</typeparam>
    public class CreatedResponse<T> : Response where T : class
    {
        private readonly string? _uri;
        private readonly T? _result;

        /// <summary>
        /// Initializes a new instance of the CreatedResponse class
        /// </summary>
        /// <param name="result">The created resource</param>
        /// <param name="uri">Optional URI where the resource can be accessed</param>
        internal CreatedResponse(T result, string? uri = null)
            : base(statusCode: HttpStatusCode.Created, success: true)
        {
            _result = result;
            _uri = uri;
        }

        /// <summary>
        /// Gets the created resource
        /// </summary>
        public T? Result => _result;

        /// <summary>
        /// Gets the URI of the created resource
        /// </summary>
        public string? Uri => _uri;
    }
}
