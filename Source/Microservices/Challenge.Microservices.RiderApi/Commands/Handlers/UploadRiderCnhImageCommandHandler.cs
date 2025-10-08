using FluentValidation;
using Challenge.Common.Core.Response.Models.Interfaces;
using Challenge.Common.Core.Validation.Extensions;
using Challenge.Common.Core.Response.Factories;
using Challenge.Microservices.RiderApi.Infra.Data.Repositories;
using Challenge.Microservices.RiderApi.Infra.Services;
using Challenge.Common.Core.Cqrs.Interfaces;

namespace Challenge.Microservices.RiderApi.Commands.Handlers
{
    /// <summary>
    /// Handles the upload of CNH (driver's license) images for riders
    /// </summary>
    public class UploadRiderCnhImageCommandHandler(
    IRiderRepository riderRepository,
    IS3Service s3Service,  // Interface, não classe concreta
    IValidator<UploadRiderCnhImageCommand> validator,
    ILogger<UploadRiderCnhImageCommandHandler> logger)
    : ICommandHandler<UploadRiderCnhImageCommand>
    {
        /// <summary>
        /// Processes the CNH image upload command
        /// </summary>
        /// <param name="command">The upload command containing rider ID and image data</param>
        /// <returns>A response indicating success or failure of the upload operation</returns>
        public async Task<IResponse> Handle(UploadRiderCnhImageCommand command)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                logger.LogWarning(
                    "Validation failed for upload CNH image. Identifier: {Identifier}",
                    command.Identifier);

                return ResponseFactory.CreateBadRequestResponse(
                    validationResult.ToErrorDictionary());
            }

            var rider = await riderRepository.GetByIdentifierAsync(command.Identifier!);
            if (rider == null)
            {
                logger.LogWarning("Rider not found. Identifier: {Identifier}", command.Identifier);
                return ResponseFactory.CreateNotFoundResponse();
            }

            try
            {
                var imageUrl = await s3Service.UploadCnhImageAsync(
                    command.CnhImage,
                    rider.Identifier,
                    rider.CnhNumber);

                rider.CnhImageUrl = imageUrl;
                await riderRepository.UpdateAsync(rider);

                logger.LogInformation(
                    "CNH image uploaded successfully. Identifier: {Identifier}, ImageUrl: {ImageUrl}",
                    rider.Id, imageUrl);

                return ResponseFactory.CreateCreatedResponse(rider);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Error uploading CNH image. Identifier: {Identifier}",
                    command.Identifier);

                return ResponseFactory.CreateCriticalResponse(ex);
            }
        }
    }
}
