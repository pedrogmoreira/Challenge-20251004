using Challenge.Common.Core.Cqrs.Interfaces;
using Challenge.Common.Core.Response.Factories;
using Challenge.Common.Core.Response.Models.Interfaces;
using Challenge.Common.Core.Validation.Extensions;
using Challenge.Microservices.MotorbikeApi.Infra.Data.Repositories;
using FluentValidation;

namespace Challenge.Microservices.MotorbikeApi.Commands.Handlers
{
    /// <summary>
    /// Handles motorbike license plate update
    /// </summary>
    public class UpdateMotorbikeLicensePlateCommandHandler(
        IMotorbikeRepository repository,
        IValidator<UpdateMotorbikeLicensePlateCommand> validator,
        ILogger<UpdateMotorbikeLicensePlateCommandHandler> logger) : ICommandHandler<UpdateMotorbikeLicensePlateCommand>
    {
        public async Task<IResponse> Handle(UpdateMotorbikeLicensePlateCommand command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                logger.LogWarning("Validation failed for update motorbike. Errors: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return ResponseFactory.CreateBadRequestResponse(validationResult.ToErrorDictionary());
            }

            var motorbike = await repository.GetByIdentifierAsync(command.Identifier!);
            if (motorbike == null)
            {
                return ResponseFactory.CreateNotFoundResponse();
            }

            try
            {
                motorbike.LicensePlate = command.LicensePlate!;
                await repository.UpdateAsync(motorbike);

                logger.LogInformation("Motorbike license plate updated. Identifier: {Identifier}", motorbike.Identifier);

                return ResponseFactory.CreateSuccessResponse(motorbike);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating motorbike license plate");
                return ResponseFactory.CreateCriticalResponse(ex);
            }
        }
    }
}