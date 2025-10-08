using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Challenge.Common.Core.Response.Models;
using Challenge.Microservices.RiderApi.Commands;
using Challenge.Common.Core.Response.Factories;
using Challenge.Microservices.RiderApi.Infra.Data.Entities;
using Challenge.Common.Core.Cqrs.Interfaces;

namespace Challenge.Microservices.RiderApi.Controllers.v1
{
    /// <summary>
    /// Controller for managing rider operations
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("entregadores")]
    [Produces("application/json")]
    public class RiderController : ControllerBase
    {
        /// <summary>
        /// Registers a new rider in the system
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/entregadores
        ///     {
        ///        "identificador": "unique-id-123",
        ///        "nome": "João Silva",
        ///        "cnpj": "12345678000190",
        ///        "data_nascimento": "1990-05-15",
        ///        "numero_cnh": "12345678901",
        ///        "tipo_cnh": "A"
        ///     }
        /// 
        /// </remarks>
        /// <param name="commandHandler">The handler for processing registration commands</param>
        /// <param name="command">The registration command with rider data</param>
        /// <returns>The created rider data</returns>
        /// <response code="201">Rider successfully registered</response>
        /// <response code="400">Invalid request data or validation errors</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreatedResponse<Rider>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CriticalResponse))]
        public async Task<IActionResult> RegisterRider(
            [FromServices] ICommandHandler<RegisterRiderCommand> commandHandler,
            [FromBody] RegisterRiderCommand command)
        {
            if (command == null)
            {
                var errors = new Dictionary<string, List<string>>
                {
                    { "Command", new List<string> { "Command cannot be empty" } }
                };
                return HttpResponseFactory.From<Rider>(
                    ResponseFactory.CreateBadRequestResponse(errors));
            }

            var response = await commandHandler.Handle(command);

            var message = response switch
            {
                SuccessResponse<Rider> success => $"Rider '{success.Result?.Name}' was successfully registered",
                _ => string.Empty
            };

            return HttpResponseFactory.From<Rider>(response, message);
        }

        /// <summary>
        /// Uploads a CNH (driver's license) image for an existing rider
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/entregadores/{id}/cnh
        ///     {
        ///        "imagem_cnh": "data:image/png;base64,iVBORw0KGgo..."
        ///     }
        /// 
        /// Accepted formats: PNG or BMP
        /// Maximum file size: 50MB
        /// 
        /// </remarks>
        /// <param name="commandHandler">The handler for processing upload commands</param>
        /// <param name="id">The unique identifier of the rider</param>
        /// <param name="command">The upload command with CNH image data in base64 format</param>
        /// <returns>The updated rider data with CNH image URL</returns>
        /// <response code="201">CNH image successfully uploaded</response>
        /// <response code="400">Invalid image format or validation errors</response>
        /// <response code="404">Rider not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("{id}/cnh")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreatedResponse<Rider>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CriticalResponse))]
        public async Task<IActionResult> UploadCnhImage(
            [FromServices] ICommandHandler<UploadRiderCnhImageCommand> commandHandler,
            [FromRoute] string id,
            [FromBody] UploadRiderCnhImageCommand command)
        {
            if (command == null)
            {
                var errors = new Dictionary<string, List<string>>
            {
                { "Command", new List<string> { "Command cannot be empty" } }
            };
                return HttpResponseFactory.From<Rider>(
                    ResponseFactory.CreateBadRequestResponse(errors));
            }

            command.Identifier = id;

            var response = await commandHandler.Handle(command);

            var message = response switch
            {
                SuccessResponse<Rider> => "CNH image uploaded successfully",
                _ => string.Empty
            };

            return HttpResponseFactory.From<Rider>(response, message);
        }
    }
}
