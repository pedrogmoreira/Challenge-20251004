using AutoMapper;
using Challenge.Common.Core.Cqrs.Interfaces;
using Challenge.Common.Core.Response.Factories;
using Challenge.Common.Core.Response.Models.Interfaces;
using Challenge.Common.Core.Validation.Extensions;
using Challenge.Microservices.MotorbikeApi.Infra.Data.Repositories;
using Challenge.Microservices.MotorbikeApi.Queries.Responses;
using FluentValidation;

namespace Challenge.Microservices.MotorbikeApi.Queries.Handlers
{
    /// <summary>
    /// Handles queries to retrieve a specific motorbike by its business identifier
    /// </summary>
    public class GetMotorbikeByIdentifierQueryHandler(
        IMapper mapper,
        IMotorbikeRepository repository, 
        ILogger<GetMotorbikeByIdentifierQueryHandler> logger,
        IValidator<GetMotorbikeByIdentifierQuery> validator) 
        : IQueryHandler<GetMotorbikeByIdentifierQuery>
    {
        /// <summary>
        /// Processes the query to retrieve a motorbike by identifier
        /// </summary>
        /// <param name="query">The query containing the motorbike identifier</param>
        /// <returns>A response containing the motorbike data or not found status</returns>
        public async Task<IResponse> Handle(GetMotorbikeByIdentifierQuery query)
        {
            var validationResult = await validator.ValidateAsync(query);
            if (!validationResult.IsValid)
            {
                logger.LogWarning("Validation failed for get motorbike by identifier. Errors: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return ResponseFactory.CreateBadRequestResponse(validationResult.ToErrorDictionary());
            }

            var motorbike = await repository.GetByIdentifierAsync(query.Identifier!);

            if (motorbike is null)
            {
                return ResponseFactory.CreateNotFoundResponse();
            }

            return ResponseFactory.CreateSuccessResponse(mapper.Map<MotorbikeResponse>(motorbike));
        }
    }
}
