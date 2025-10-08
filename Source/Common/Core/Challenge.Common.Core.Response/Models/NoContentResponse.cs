using System.Net;

namespace Challenge.Common.Core.Response.Models
{
    /// <summary>
    /// Response indicating successful operation with no content to return (204)
    /// </summary>
    /// <param name="success">Indicates if the operation was successful</param>
    public class NoContentResponse(bool success)
        : Response(statusCode: HttpStatusCode.NoContent, success: success)
    {
    }
}
