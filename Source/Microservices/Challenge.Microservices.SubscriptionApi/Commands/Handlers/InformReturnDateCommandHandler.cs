using Challenge.Common.Core.Cqrs.Interfaces;
using Challenge.Common.Core.Response.Factories;
using Challenge.Common.Core.Response.Models.Interfaces;
using Challenge.Common.Core.Validation.Extensions;
using Challenge.Microservices.SubscriptionApi.Infra.Data.Entities;
using Challenge.Microservices.SubscriptionApi.Infra.Data.Repositories;
using Challenge.Microservices.SubscriptionApi.Infra.Services;
using FluentValidation;

namespace Challenge.Microservices.SubscriptionApi.Commands.Handlers
{
    /// <summary>
    /// Handles return date information and cost calculation
    /// </summary>
    public class InformReturnDateCommandHandler(
        ISubscriptionRepository repository,
        IValidator<InformReturnDateCommand> validator,
        ILogger<InformReturnDateCommandHandler> logger,
        ISubscriptionCostService costService) : ICommandHandler<InformReturnDateCommand>
    {
        public async Task<IResponse> Handle(InformReturnDateCommand command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                logger.LogWarning("Validation failed for inform return date. Errors: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return ResponseFactory.CreateBadRequestResponse(validationResult.ToErrorDictionary());
            }

            var subscription = await repository.GetByIdentifierAsync(command.SubscriptionIdentifier!);
            if (subscription == null)
            {
                return ResponseFactory.CreateNotFoundResponse();
            }

            if (subscription.Status != SubscriptionStatus.Active)
            {
                var errors = ValidationExtensions.SingleError(
                    "Subscription",
                    "Subscription is not active");
                return ResponseFactory.CreateBadRequestResponse(errors);
            }

            try
            {
                // Calculate cost
                var costResponse = costService.CalculateCost(subscription, command.ReturnDate);

                // Update subscription
                subscription.ActualReturnDate = command.ReturnDate;
                subscription.TotalCost = costResponse.TotalCost;
                subscription.Status = SubscriptionStatus.Completed;

                await repository.UpdateAsync(subscription);

                logger.LogInformation(
                    "Subscription return processed. SubscriptionId: {SubscriptionId}, TotalCost: {TotalCost}",
                    subscription.Id, costResponse.TotalCost);

                return ResponseFactory.CreateSuccessResponse(costResponse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing return date");
                return ResponseFactory.CreateCriticalResponse(ex);
            }
        }
    }
}