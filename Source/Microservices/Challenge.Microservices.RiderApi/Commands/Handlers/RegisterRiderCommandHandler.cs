using AutoMapper;
using Challenge.Common.Core.Response.Models.Interfaces;
using Challenge.Common.Core.Validation.Extensions;
using FluentValidation;
using Challenge.Common.Core.Response.Factories;
using Challenge.Microservices.RiderApi.Infra.Data.Entities;
using Challenge.Microservices.RiderApi.Infra.Data.Repositories;
using Challenge.Common.Core.Cqrs.Interfaces;

namespace Challenge.Microservices.RiderApi.Commands.Handlers
{
    /// <summary>
    /// Handles the registration of a new rider in the system
    /// </summary>
    public class RegisterRiderCommandHandler(
    IMapper mapper,
    IRiderRepository riderRepository,
    IValidator<RegisterRiderCommand> validator,
    ILogger<RegisterRiderCommandHandler> logger)
    : ICommandHandler<RegisterRiderCommand>
    {
        /// <summary>
        /// Processes the rider registration command
        /// </summary>
        /// <param name="command">The registration command containing rider information</param>
        /// <returns>A response indicating success or failure of the operation</returns>
        public async Task<IResponse> Handle(RegisterRiderCommand command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                logger.LogWarning(
                    "Validation failed for register rider. Errors: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                return ResponseFactory.CreateBadRequestResponse(
                    validationResult.ToErrorDictionary());
            }

            try
            {
                var rider = mapper.Map<Rider>(command);

                await riderRepository.AddAsync(rider);

                logger.LogInformation(
                    "Rider registered successfully. Identifier: {Identifier}, Cnpj: {Cnpj}",
                    command.Identifier, rider.Cnpj);

                return ResponseFactory.CreateCreatedResponse(rider);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Error registering rider. Cnpj: {Cnpj}",
                    command.Cnpj);

                return ResponseFactory.CreateCriticalResponse(ex);
            }
        }
    }
}
