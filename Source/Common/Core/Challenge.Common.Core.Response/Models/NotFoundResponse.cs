using System.Net;
namespace Challenge.Common.Core.Response.Models
{
    /// <summary>
    /// Response indicating the requested resource was not found (404)
    /// </summary>
    public class NotFoundResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the NotFoundResponse class
        /// </summary>
        public NotFoundResponse()
            : base(statusCode: HttpStatusCode.NotFound, success: false) { }
    }
}
