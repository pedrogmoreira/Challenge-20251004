using Challenge.Common.Core.Response.Models;

namespace Challenge.Common.Core.Response.Factories
{
    /// <summary>
    /// Factory for creating standardized response objects
    /// </summary>
    public static class ResponseFactory
    {
        /// <summary>
        /// Creates a bad request response with validation errors
        /// </summary>
        /// <param name="errors">Dictionary of field names and their error messages</param>
        /// <returns>A bad request response (400)</returns>
        public static BadRequestResponse CreateBadRequestResponse(Dictionary<string, List<string>> errors)
        {
            return new BadRequestResponse(errors);
        }

        /// <summary>
        /// Creates a conflict response indicating resource conflict
        /// </summary>
        /// <param name="errors">Dictionary of field names and their conflict messages</param>
        /// <returns>A conflict response (409)</returns>
        public static ConflictResponse CreateConflictResponse(Dictionary<string, List<string>> errors)
        {
            return new ConflictResponse(errors);
        }

        /// <summary>
        /// Creates a created response for successful resource creation
        /// </summary>
        /// <typeparam name="T">The type of the created resource</typeparam>
        /// <param name="result">The created resource</param>
        /// <param name="uri">Optional URI of the created resource</param>
        /// <returns>A created response (201)</returns>
        public static CreatedResponse<T> CreateCreatedResponse<T>(T result, string? uri = null) where T : class
        {
            return new CreatedResponse<T>(result, uri);
        }

        /// <summary>
        /// Creates a critical response for unhandled server errors
        /// </summary>
        /// <param name="exception">The exception that occurred</param>
        /// <returns>A critical error response (500)</returns>
        public static CriticalResponse CreateCriticalResponse(Exception exeption)
        {
            return new CriticalResponse(exeption);
        }

        /// <summary>
        /// Creates a no content response for successful operations with no return value
        /// </summary>
        /// <param name="success">Indicates if the operation was successful</param>
        /// <returns>A no content response (204)</returns>
        public static NoContentResponse CreateNoContentResponse(bool success)
        {
            return new NoContentResponse(success);
        }

        /// <summary>
        /// Creates a not found response when requested resource doesn't exist
        /// </summary>
        /// <returns>A not found response (404)</returns>
        public static NotFoundResponse CreateNotFoundResponse()
        {
            return new NotFoundResponse();
        }

        /// <summary>
        /// Creates a success response with the result data
        /// </summary>
        /// <typeparam name="T">The type of the result data</typeparam>
        /// <param name="result">The result data to return</param>
        /// <returns>A success response (200)</returns>
        public static SuccessResponse<T> CreateSuccessResponse<T>(T result) where T : class
        {
            return new SuccessResponse<T>(result);
        }

        /// <summary>
        /// Creates an unauthorized response for authentication/authorization failures
        /// </summary>
        /// <param name="errors">Optional dictionary of error messages</param>
        /// <returns>An unauthorized response (401)</returns>
        public static UnauthorizedResponse CreateUnauthorizedResponse(Dictionary<string, List<string>>? errors = null)
        {
            return new UnauthorizedResponse(errors);
        }
    }
}
