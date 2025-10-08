using Challenge.Common.Core.Response.Models.Interfaces;
using System.Net;

namespace Challenge.Common.Core.Response.Models
{
    /// <summary>
    /// Abstract base class for all response types
    /// </summary>
    public abstract class Response : IResponse
    {
        private readonly HttpStatusCode _statusCode;
        private readonly bool _success;
        private string _message;

        /// <summary>
        /// Initializes a new instance of the Response class
        /// </summary>
        /// <param name="statusCode">The HTTP status code</param>
        /// <param name="success">Indicates if the operation was successful</param>
        /// <param name="message">Optional message to include in the response</param>
        internal Response(HttpStatusCode statusCode, bool success, string message = "")
        {
            _statusCode = statusCode;
            _success = success;
            _message = message;
        }

        /// <summary>
        /// Sets a custom message for the response
        /// </summary>
        /// <param name="message">The message to set</param>
        public void SetMessage(string message) => _message = message;

        /// <summary>
        /// Gets the HTTP status code for this response
        /// </summary>
        /// <returns>The HTTP status code</returns>
        public HttpStatusCode GetStatusCode() => _statusCode;


        /// <summary>
        /// Gets the response message
        /// </summary>
        public string Message => _message;

        /// <summary>
        /// Gets whether the operation was successful
        /// </summary>
        public bool Success => _success;
    }
}
