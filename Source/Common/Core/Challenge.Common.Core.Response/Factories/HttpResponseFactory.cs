using Challenge.Common.Core.Response.Exceptions;
using Challenge.Common.Core.Response.Models.Interfaces;
using Challenge.Common.Core.Response.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Common.Core.Response.Factories
{
    /// <summary>
    /// Factory for converting response objects to HTTP action results
    /// </summary>
    public static class HttpResponseFactory
    {
        /// <summary>
        /// Converts a response object to the appropriate HTTP action result
        /// </summary>
        /// <typeparam name="T">The type of data in the response</typeparam>
        /// <param name="response">The response object to convert</param>
        /// <param name="message">Optional custom message to include in the response</param>
        /// <returns>An IActionResult representing the HTTP response</returns>
        public static IActionResult From<T>(IResponse response, string message = "") where T : class
        {
            return response switch
            {
                BadRequestResponse badRequest => CreateBadRequestResponse(badRequest, message),
                ConflictResponse conflict => CreateConflictResponse(conflict, message),
                CreatedResponse<T> created => CreateCreatedResponse(created, message),
                CriticalResponse critical => CreateInternalServerErrorResponse(critical, message),
                NotFoundResponse => CreateNotFoundResponse(),
                NoContentResponse => CreateNoContentResponse(),
                SuccessResponse<T> success => CreateOkResponse(success, message),
                UnauthorizedResponse unauthorized => CreateUnauthorizedResponse(unauthorized, message),
                _ => CreateInternalServerErrorResponse()
            };
        }

        /// <summary>
        /// Creates a bad request action result (400)
        /// </summary>
        /// <param name="badRequestResponse">The bad request response</param>
        /// <param name="message">Optional custom message</param>
        /// <returns>A BadRequestObjectResult</returns>
        private static BadRequestObjectResult CreateBadRequestResponse(BadRequestResponse badRequestResponse, string message)
        {
            badRequestResponse.SetMessage(message);
            return new BadRequestObjectResult(badRequestResponse);
        }

        // <summary>
        /// Creates a conflict action result (409)
        /// </summary>
        /// <param name="conflictResponse">The conflict response</param>
        /// <param name="message">Optional custom message</param>
        /// <returns>A ConflictResult or ObjectResult with 409 status</returns>
        private static IActionResult CreateConflictResponse(ConflictResponse conflictResponse, string message)
        {
            conflictResponse.SetMessage(message);
            if (conflictResponse.Errors.Count == 0)
            {
                return new ConflictResult();
            }

            return new ObjectResult(conflictResponse)
            {
                StatusCode = StatusCodes.Status409Conflict
            };
        }

        /// <summary>
        /// Creates an unauthorized action result (401)
        /// </summary>
        /// <param name="unauthorizedResponse">The unauthorized response</param>
        /// <param name="message">Optional custom message</param>
        /// <returns>An UnauthorizedResult or ObjectResult with 401 status</returns>
        private static IActionResult CreateUnauthorizedResponse(UnauthorizedResponse unauthorizedResponse, string message)
        {
            unauthorizedResponse.SetMessage(message);
            if (unauthorizedResponse.Errors.Count == 0)
            {
                return new UnauthorizedResult();
            }

            return new ObjectResult(unauthorizedResponse)
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };

        }

        /// <summary>
        /// Creates a created action result (201)
        /// </summary>
        /// <typeparam name="T">The type of the created resource</typeparam>
        /// <param name="createdResponse">The created response</param>
        /// <param name="message">Optional custom message</param>
        /// <returns>A CreatedResult with the resource URI</returns>
        private static CreatedResult CreateCreatedResponse<T>(CreatedResponse<T> createdResponse, string message) where T : class
        {
            createdResponse.SetMessage(message);
            return new CreatedResult(createdResponse.Uri, createdResponse);
        }

        /// <summary>
        /// Creates a success action result (200)
        /// </summary>
        /// <typeparam name="T">The type of the result data</typeparam>
        /// <param name="successResponse">The success response</param>
        /// <param name="message">Optional custom message</param>
        /// <returns>An OkObjectResult with the data</returns>
        public static OkObjectResult CreateOkResponse<T>(SuccessResponse<T> successResponse, string message) where T : class
        {
            successResponse.SetMessage(message);
            return new OkObjectResult(successResponse);
        }

        /// <summary>
        /// Creates an internal server error action result (500) with exception details
        /// </summary>
        /// <param name="criticalResponse">The critical response containing exception information</param>
        /// <param name="message">Optional custom message</param>
        /// <returns>An ObjectResult with 500 status</returns>
        public static ObjectResult CreateInternalServerErrorResponse(CriticalResponse criticalResponse, string message)
        {
            criticalResponse.SetMessage(message);
            return new ObjectResult(criticalResponse)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        /// <summary>
        /// Creates a default internal server error action result (500)
        /// </summary>
        /// <returns>An ObjectResult with 500 status and unhandled exception</returns>
        public static ObjectResult CreateInternalServerErrorResponse()
        {
            return new ObjectResult(ResponseFactory.CreateCriticalResponse(new UnhandledInternalException()))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        /// <summary>
        /// Creates a not found action result (404)
        /// </summary>
        /// <returns>A NotFoundResult</returns>
        public static IActionResult CreateNotFoundResponse()
        {
            return new NotFoundResult();
        }

        /// <summary>
        /// Creates a no content action result (204)
        /// </summary>
        /// <returns>A NotFoundResult</returns>
        public static IActionResult CreateNoContentResponse()
        {
            return new NoContentResult();
        }
    }
}
