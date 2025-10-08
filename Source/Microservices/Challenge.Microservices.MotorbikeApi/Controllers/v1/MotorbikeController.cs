using Asp.Versioning;
using Challenge.Common.Core.Cqrs.Interfaces;
using Challenge.Common.Core.Response.Factories;
using Challenge.Common.Core.Response.Models;
using Challenge.Microservices.MotorbikeApi.Commands;
using Challenge.Microservices.MotorbikeApi.Infra.Data.Entities;
using Challenge.Microservices.MotorbikeApi.Queries;
using Challenge.Microservices.MotorbikeApi.Queries.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Microservices.MotorbikeApi.Controllers.v1
{
    /// <summary>
    /// Controller for managing motorbike operations
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("motos")]
    [Produces("application/json")]
    public class MotorbikeController : ControllerBase
    {
        /// <summary>
        /// Registers a new motorbike in the system and publishes a registration event
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/motos
        ///     {
        ///        "identificador": "bike-001",
        ///        "ano": 2024,
        ///        "modelo": "Honda CG 160",
        ///        "placa": "ABC1D23"
        ///     }
        /// 
        /// Business rules:
        /// - License plate must be unique
        /// - Publishes event via messaging for 2024 year motorbikes
        /// 
        /// </remarks>
        /// <param name="handler">The handler for processing motorbike registration</param>
        /// <param name="command">The registration command with motorbike data</param>
        /// <returns>The created motorbike data</returns>
        /// <response code="201">Motorbike successfully registered</response>
        /// <response code="400">Invalid request data or validation errors</response>
        /// <response code="409">License plate already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreatedResponse<Motorbike>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResponse))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ConflictResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CriticalResponse))]
        public async Task<IActionResult> RegisterMotorbike(
            [FromServices] ICommandHandler<RegisterMotorbikeCommand> handler,
            [FromBody] RegisterMotorbikeCommand command)
        {
            if (command == null)
            {
                var errors = new Dictionary<string, List<string>>
                {
                    { "Command", new List<string> { "Command cannot be empty" } }
                };
                return HttpResponseFactory.From<Motorbike>(
                    ResponseFactory.CreateBadRequestResponse(errors));
            }

            var response = await handler.Handle(command);
            return HttpResponseFactory.From<Motorbike>(response);
        }

        /// <summary>
        /// Retrieves all motorbikes with optional filtering by license plate
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/motos
        ///     GET /api/v1/motos?placa=ABC1D23
        /// 
        /// </remarks>
        /// <param name="handler">The query handler</param>
        /// <param name="licensePlate">Optional license plate filter (exact match)</param>
        /// <returns>List of motorbikes matching the criteria</returns>
        /// <response code="200">Motorbikes retrieved successfully</response>
        /// <response code="404">No motorbikes found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<IEnumerable<Motorbike>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CriticalResponse))]
        public async Task<IActionResult> GetMotorbikes(
            [FromServices] IQueryHandler<GetMotorbikesQuery> handler,
            [FromQuery(Name = "placa")] string? licensePlate)
        {
            var query = new GetMotorbikesQuery { LicensePlate = licensePlate };

            var response = await handler.Handle(query);
            return HttpResponseFactory.From<IEnumerable<MotorbikeResponse>>(response);
        }

        /// <summary>
        /// Retrieves a specific motorbike by its business identifier
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/motos/{id}
        /// 
        /// </remarks>
        /// <param name="handler">The query handler</param>
        /// <param name="id">The unique business identifier of the motorbike</param>
        /// <returns>The motorbike data if found</returns>
        /// <response code="200">Motorbike found and returned successfully</response>
        /// <response code="404">Motorbike not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<IEnumerable<Motorbike>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CriticalResponse))]
        public async Task<IActionResult> GetMotorbike(
            [FromServices] IQueryHandler<GetMotorbikeByIdentifierQuery> handler,
            [FromRoute] string id)
        {
            var query = new GetMotorbikeByIdentifierQuery { Identifier = id };

            var response = await handler.Handle(query);
            return HttpResponseFactory.From<MotorbikeResponse>(response);
        }

        /// <summary>
        /// Updates the license plate of an existing motorbike (for incorrectly registered plates)
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/v1/motos/{id}/placa
        ///     {
        ///        "placa": "XYZ9W87"
        ///     }
        /// 
        /// Business rules:
        /// - New license plate must be unique
        /// - Only updates the license plate field
        /// 
        /// </remarks>
        /// <param name="handler">The command handler</param>
        /// <param name="id">The motorbike identifier</param>
        /// <param name="command">The update command with motorbike data</param>
        /// <returns>The updated motorbike data</returns>
        /// <response code="200">License plate updated successfully</response>
        /// <response code="400">Invalid license plate format</response>
        /// <response code="404">Motorbike not found</response>
        /// <response code="409">New license plate already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}/placa")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<Motorbike>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResponse))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ConflictResponse))]
        public async Task<IActionResult> UpdateLicensePlate(
            [FromServices] ICommandHandler<UpdateMotorbikeLicensePlateCommand> handler,
            [FromRoute] string id,
            [FromBody] UpdateMotorbikeLicensePlateCommand command)
        {
            command.Identifier = id;

            var response = await handler.Handle(command);
            return HttpResponseFactory.From<Motorbike>(response);
        }

        /// <summary>
        /// Disables a motorbike (only if it has no rental history)
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /api/v1/motos/{id}
        /// 
        /// Business rules:
        /// - Cannot disable motorbikes with existing rental records
        /// - Virtual deletion operation
        /// 
        /// </remarks>
        /// <param name="handler">The command handler</param>
        /// <param name="id">The motorbike identifier to delete</param>
        /// <returns>No content on success</returns>
        /// <response code="204">Motorbike deleted successfully</response>
        /// <response code="400">Cannot delete - motorbike has rental history</response>
        /// <response code="404">Motorbike not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMotorbike(
            [FromServices] ICommandHandler<DeleteMotorbikeCommand> handler,
            [FromRoute] string id)
        {
            var command = new DeleteMotorbikeCommand { Identifier = id };
            var response = await handler.Handle(command);
            return HttpResponseFactory.From<Motorbike>(response);
        }
    }
}