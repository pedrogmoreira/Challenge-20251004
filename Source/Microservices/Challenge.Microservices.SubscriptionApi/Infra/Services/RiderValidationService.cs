using Challenge.Common.Core.Response.Models;
using Challenge.Common.Messaging.Grpc;
using Challenge.Microservices.SubscriptionApi.Infra.Services.Response;
using Grpc.Core;

namespace Challenge.Microservices.SubscriptionApi.Infra.Services
{

    /// <summary>
    /// Implementation using gRPC to communicate with RiderApi
    /// </summary>
    public class RiderValidationService(
        RiderService.RiderServiceClient grpcClient,
        ILogger<RiderValidationService> logger) : IRiderValidationService
    {
        public async Task<HasValidVicenseResponse> HasValidLicenseAsync(string riderIdentifier)
        {
            try
            {
                var request = new ValidateRiderRequest
                {
                    RiderIdentifier = riderIdentifier
                };

                var response = await grpcClient.ValidateRiderCnhTypeAsync(
                    request,
                    deadline: DateTime.UtcNow.AddSeconds(5));

                if (!response.IsValid)
                {
                    logger.LogWarning(
                        "Rider validation failed. RiderIdentifier: {RiderIdentifier}, Message: {Message}",
                        riderIdentifier, response.Message);
                }

                return new HasValidVicenseResponse
                {
                    IsValid = response.IsValid,
                    Message = response.Message
                };
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                logger.LogWarning("Rider not found. RiderIdentifier: {RiderIdentifier}", riderIdentifier);
                return new HasValidVicenseResponse
                {
                    IsValid = false,
                    Message = $"Rider not found. RiderIdentifier: {riderIdentifier}",
                };
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
            {
                logger.LogError("Timeout validating rider. RiderIdentifier: {RiderIdentifier}", riderIdentifier);
                return new HasValidVicenseResponse
                {
                    IsValid = false,
                    Message = $"Timeout validating rider. RiderIdentifier: {riderIdentifier}",
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error validating rider. RiderIdentifier: {RiderIdentifier}", riderIdentifier);
                return new HasValidVicenseResponse
                {
                    IsValid = false,
                    Message = $"Error validating rider. RiderIdentifier: {riderIdentifier}",
                };
            }
        }
    }
}