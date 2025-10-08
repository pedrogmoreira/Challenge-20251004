using AutoMapper;
using Challenge.Common.Core.Cqrs.Interfaces;
using Challenge.Common.Core.Response.Factories;
using Challenge.Common.Core.Response.Models.Interfaces;
using Challenge.Common.Core.Validation.Extensions;
using Challenge.Microservices.SubscriptionApi.Infra.Data.Repositories;
using Challenge.Microservices.SubscriptionApi.Queries.Responses;
using FluentValidation;

namespace Challenge.Microservices.SubscriptionApi.Queries.Handlers
{
    public class GetSubscriptionQueryHandler(
        IMapper mapper,
        ISubscriptionRepository repository,
        ILogger<GetSubscriptionQueryHandler> logger,
        IValidator<GetSubscriptionQuery> validator) 
        : IQueryHandler<GetSubscriptionQuery>
    {
        /// <summary>
        /// Processes the query to retrieve a subscription by its identifier.
        /// </summary>
        /// <param name="query">The query containing filter</param>
        /// <returns>A response containing the list of motorbikes</returns>
        public async Task<IResponse> Handle(GetSubscriptionQuery query)
        {
            var validationResult = await validator.ValidateAsync(query);
            if (!validationResult.IsValid)
            {
                logger.LogWarning("Validation failed for get subscription.Errors: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return ResponseFactory.CreateBadRequestResponse(validationResult.ToErrorDictionary());
            }

            var subscription = await repository.GetByIdentifierAsync(query.Identifier!);

            if (subscription is null)
            {
                return ResponseFactory.CreateNotFoundResponse();
            }
            else
            {                
                var data = mapper.Map<SubscriptionResponse>(subscription);
                return ResponseFactory.CreateSuccessResponse(data);
            }
        }
    }
}
