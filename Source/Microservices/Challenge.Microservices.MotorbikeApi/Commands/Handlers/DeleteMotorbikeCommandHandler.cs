using Challenge.Common.Core.Cqrs.Interfaces;
using Challenge.Common.Core.Response.Factories;
using Challenge.Common.Core.Response.Models.Interfaces;
using Challenge.Common.Core.Validation.Extensions;
using Challenge.Microservices.MotorbikeApi.Infra.Data.Repositories;
using FluentValidation;

namespace Challenge.Microservices.MotorbikeApi.Commands.Handlers
{
    /// <summary>
    /// Handles motorbike deletion
    /// </summary>
    public class DeleteMotorbikeCommandHandler(
        IMotorbikeRepository repository,
        IValidator<DeleteMotorbikeCommand> validator,
        ILogger<DeleteMotorbikeCommandHandler> logger) : ICommandHandler<DeleteMotorbikeCommand>
    {

        public async Task<IResponse> Handle(DeleteMotorbikeCommand command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                logger.LogWarning("Validation failed for delete motorbike. Errors: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return ResponseFactory.CreateBadRequestResponse(validationResult.ToErrorDictionary());
            }

            var motorbike = await repository.GetByIdentifierAsync(command.Identifier);
            if (motorbike == null)
            {
                return ResponseFactory.CreateNotFoundResponse();
            }

            // Check if motorbike has active rentals
            var hasActiveRentals = await repository.HasActiveRentalsAsync(command.Identifier);
            if (hasActiveRentals)
            {
                var errors = ValidationExtensions.SingleError("Motorbike", "Cannot delete motorbike with active rentals");
                return ResponseFactory.CreateBadRequestResponse(errors);
            }

            try
            {
                await repository.DisableByIdentifierAsync(command.Identifier);

                logger.LogInformation("Motorbike disabled successfully. Identifier: {Identifier}", command.Identifier);

                return ResponseFactory.CreateNoContentResponse(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting motorbike");
                return ResponseFactory.CreateCriticalResponse(ex);
            }
        }
    }
}