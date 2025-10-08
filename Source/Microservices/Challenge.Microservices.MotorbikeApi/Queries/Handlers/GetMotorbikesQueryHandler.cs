using AutoMapper;
using Challenge.Common.Core.Collections.Extensions;
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
    /// Handles queries to retrieve a list of motorbikes with optional lisence plate filtering
    /// </summary>
    public class GetMotorbikesQueryHandler(
        IMapper mapper,
        IMotorbikeRepository repository,
        ILogger<GetMotorbikesQueryHandler> logger,
        IValidator<GetMotorbikesQuery> validator)
        : IQueryHandler<GetMotorbikesQuery>
    {
        /// <summary>
        /// Processes the query to retrieve motorbikes with optional license plate filter
        /// </summary>
        /// <param name="query">The query containing optional filter</param>
        /// <returns>A response containing the list of motorbikes</returns>
        public async Task<IResponse> Handle(GetMotorbikesQuery query)
        {
            var validationResult = await validator.ValidateAsync(query);
            if (!validationResult.IsValid)
            {
                logger.LogWarning("Validation failed for get motorbike. Errors: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return ResponseFactory.CreateBadRequestResponse(validationResult.ToErrorDictionary());
            }

            var motorbikes = repository.GetActive();

            if (motorbikes.IsNullOrEmpty())
            {
                return ResponseFactory.CreateNotFoundResponse();
            }
            else
            {
                motorbikes = query.LicensePlate != null
                    ? motorbikes?.Where(m => m.LicensePlate!.Equals(query.LicensePlate, StringComparison.OrdinalIgnoreCase)) ?? []
                    : motorbikes;
                var data = mapper.Map<IEnumerable<MotorbikeResponse>>(motorbikes);
                return ResponseFactory.CreateSuccessResponse(data);
            }                
        }
    }
}
