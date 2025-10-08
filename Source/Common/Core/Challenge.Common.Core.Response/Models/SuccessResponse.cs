using System.Net;

namespace Challenge.Common.Core.Response.Models
{
    /// <summary>
    /// Response indicating a successful operation with result data (200)
    /// </summary>
    /// <typeparam name="T">The type of the result data</typeparam>
    public class SuccessResponse<T> : Response where T : class
    {
        private readonly T? _result;

        /// <summary>
        /// Initializes a new instance of the SuccessResponse class
        /// </summary>
        /// <param name="result">The result data to return</param>
        internal SuccessResponse(T result)
            : base(statusCode: HttpStatusCode.OK, success: true)
        {
            _result = result;
        }

        /// <summary>
        /// Gets the result data
        /// </summary>
        public T Result => _result!;
    }
}
