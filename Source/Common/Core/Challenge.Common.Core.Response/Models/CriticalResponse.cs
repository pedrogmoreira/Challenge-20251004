using System.Net;

namespace Challenge.Common.Core.Response.Models
{
    /// <summary>
    /// Response indicating a critical internal server error (500)
    /// </summary>
    /// <param name="exception">The exception that caused the critical error</param>
    public class CriticalResponse(Exception exception)
        : Response(statusCode: HttpStatusCode.InternalServerError, success: false)
    {
        private readonly Exception? _exception = exception;

        /// <summary>
        /// Gets the exception that caused the error
        /// </summary>
        /// <returns>The exception object</returns>
        public Exception? GetException() => _exception;

        /// <summary>
        /// Gets the exception message
        /// </summary>
        public string? ExceptionMessage => _exception?.Message;

        /// <summary>
        /// Gets the inner exception message if available
        /// </summary>
        public string? InnerExceptionMessage => _exception?.InnerException?.Message;

        /// <summary>
        /// Gets the source of the exception
        /// </summary>
        public string? Source => _exception?.Source;
    }
}
