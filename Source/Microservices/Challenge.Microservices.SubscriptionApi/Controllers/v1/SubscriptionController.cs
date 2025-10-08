using Asp.Versioning;
using Challenge.Common.Core.Cqrs.Interfaces;
using Challenge.Common.Core.Response.Factories;
using Challenge.Common.Core.Response.Models;
using Challenge.Microservices.SubscriptionApi.Commands;
using Challenge.Microservices.SubscriptionApi.Infra.Data.Entities;
using Challenge.Microservices.SubscriptionApi.Infra.Services.Response;
using Challenge.Microservices.SubscriptionApi.Queries;
using Challenge.Microservices.SubscriptionApi.Queries.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Microservices.SubscriptionApi.Controllers.v1
{
    /// <summary>
    /// Controller for managing rental subscriptions
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("locacao")]
    [Produces("application/json")]
    public class SubscriptionController : ControllerBase
    {
        /// <summary>
        /// Creates a new rental subscription
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /locacao
        ///     {
        ///        "entregador_id": "507f1f77bcf86cd799439011",
        ///        "moto_id": "507f191e810c19729de860ea",
        ///        "data_inicio": "2025-01-10",
        ///        "data_termino": "2025-01-17",
        ///        "data_previsao_termino": "2025-01-17",
        ///        "plano": 7
        ///     }
        /// 
        /// Valid plans: 7, 15, 30, 45, or 50 days
        /// Only riders with category 'A' or 'AB' license can rent
        /// </remarks>
        /// <param name="handler">The handler for processing subscription creation</param>
        /// <param name="command">The command containing rental details</param>
        /// <returns>The created subscription data</returns>
        /// <response code="201">Subscription successfully created</response>
        /// <response code="400">Invalid request data or validation errors</response>
        /// <response code="409">Conflict - rider or motorbike not available for rental</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreatedResponse<Subscription>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResponse))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ConflictResponse))]
        public async Task<IActionResult> CreateSubscription(
            [FromServices] ICommandHandler<CreateSubscriptionCommand> handler,
            [FromBody] CreateSubscriptionCommand command)
        {
            if (command == null)
            {
                var errors = new Dictionary<string, List<string>>
                {
                    { "Command", new List<string> { "Command cannot be empty" } }
                };
                return HttpResponseFactory.From<Subscription>(
                    ResponseFactory.CreateBadRequestResponse(errors));
            }

            var response = await handler.Handle(command);
            return HttpResponseFactory.From<Subscription>(response);
        }

        /// <summary>
        /// Informs the return date and calculates the total cost
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /locacao/{id}/devolucao
        ///     {
        ///        "data_devolucao": "2025-01-15"
        ///     }
        /// 
        /// Cost calculation rules:
        /// - Early return: charges used days + penalty on unused days (20% for 7-day plan, 40% for 15-day plan)
        /// - Late return: charges all days + R$50 per additional day
        /// - On-time return: charges only the daily rate
        /// </remarks>
        /// <param name="handler">The handler for processing return date commands</param>
        /// <param name="id">The unique identifier of the subscription</param>
        /// <param name="command">The command containing the return date</param>
        /// <returns>The calculated total cost with breakdown of fees and penalties</returns>
        /// <response code="200">Return date processed and cost calculated successfully</response>
        /// <response code="400">Invalid request data or validation errors</response>
        /// <response code="404">Subscription not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}/devolucao")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<SubscriptionCostResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> InformReturnDate(
            [FromServices] ICommandHandler<InformReturnDateCommand> handler,
            [FromRoute] string id,
            [FromBody] InformReturnDateCommand command)
        {
            command.SubscriptionIdentifier = id;

            var response = await handler.Handle(command);
            return HttpResponseFactory.From<SubscriptionCostResponse>(response);
        }

        /// <summary>
        /// Retrieves a subscription/rental by its unique identifier
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/locacao/{id}
        /// 
        /// </remarks>
        /// <param name="handler">The handler for processing subscription queries</param>
        /// <param name="id">The unique identifier of the subscription</param>
        /// <returns>The subscription details if found</returns>
        /// <response code="200">Subscription successfully retrieved</response>
        /// <response code="404">Subscription not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<Subscription>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(CriticalResponse))]
        public async Task<IActionResult> GetSubscription(
            [FromServices] IQueryHandler<GetSubscriptionQuery> handler,
            [FromRoute] string id)
        {
            var query = new GetSubscriptionQuery { Identifier = id };

            var response = await handler.Handle(query);
            return HttpResponseFactory.From<SubscriptionResponse>(response);
        }
    }
}