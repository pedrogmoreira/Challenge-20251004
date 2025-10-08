using AutoMapper;
using Challenge.Common.Core.Cqrs.Interfaces;
using Challenge.Common.Core.Response.Factories;
using Challenge.Common.Core.Response.Models.Interfaces;
using Challenge.Common.Core.Validation.Extensions;
using Challenge.Microservices.MotorbikeApi.Infra.Data.Entities;
using Challenge.Microservices.MotorbikeApi.Infra.Data.Repositories;
using Challenge.Microservices.MotorbikeApi.Infra.Messaging.Publishers;
using FluentValidation;

namespace Challenge.Microservices.MotorbikeApi.Commands.Handlers
{
    /// <summary>
    /// Handles motorbike registration
    /// </summary>
    public class RegisterMotorbikeCommandHandler(
        IMapper mapper,
        IMotorbikeRepository repository,
        IValidator<RegisterMotorbikeCommand> validator,
        ILogger<RegisterMotorbikeCommandHandler> logger,
        IMotorbikeMessagePublisher messagePublisher) : ICommandHandler<RegisterMotorbikeCommand>
    {
        public async Task<IResponse> Handle(RegisterMotorbikeCommand command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                logger.LogWarning("Validation failed for register motorbike. Errors: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return ResponseFactory.CreateBadRequestResponse(validationResult.ToErrorDictionary());
            }

            try
            {
                var motorbike = mapper.Map<Motorbike>(command);

                await repository.AddAsync(motorbike);

                // Publish message to RabbitMQ
                await messagePublisher.PublishMotorbikeRegisteredAsync(motorbike);

                logger.LogInformation("Motorbike registered successfully. Identifier: {Identifier}", command.Identifier);

                return ResponseFactory.CreateCreatedResponse(motorbike);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error registering motorbike");
                return ResponseFactory.CreateCriticalResponse(ex);
            }
        }
    }
}