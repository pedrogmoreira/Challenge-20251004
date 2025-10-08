using AutoMapper;
using Challenge.Common.Messaging.Grpc;
using Challenge.Microservices.RiderApi.Infra.Data.Repositories;
using Grpc.Core;

namespace Challenge.Microservices.RiderApi.Infra.Services
{
    /// <summary>
    /// gRPC service implementation for rider operations
    /// </summary>
    public class RiderGrpcService(
        IMapper mapper,
        IRiderRepository riderRepository,
        ILogger<RiderGrpcService> logger) : RiderService.RiderServiceBase
    {
        private const string REQUIRED_CNH_TYPE = "A";

        /// <summary>
        /// Validates if rider has the required CNH type
        /// </summary>
        public override async Task<ValidateRiderResponse> ValidateRiderCnhType(
            ValidateRiderRequest request,
            ServerCallContext context)
        {
            try
            {
                var rider = await riderRepository.GetByIdentifierAsync(request.RiderIdentifier);

                if (rider == null || !rider.Active)
                {
                    return new ValidateRiderResponse
                    {
                        IsValid = false,
                        Message = "Rider not found or inactive"
                    };
                }

                // Check if CNH type matches
                // A rider with "AB" can also rent (contains "A")
                logger.LogInformation("cnh: {CNH}", rider.CnhType);
                var hasRequiredType = rider.CnhType.Contains(REQUIRED_CNH_TYPE,
                    StringComparison.OrdinalIgnoreCase);

                return new ValidateRiderResponse
                {
                    IsValid = hasRequiredType,
                    CnhType = rider.CnhType,
                    Message = hasRequiredType
                        ? "Rider has required CNH type"
                        : $"Rider CNH type '{rider.CnhType}' does not include 'A' or 'AB'"
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error validating rider CNH type. RiderIdentifier: {RiderIdentifier}", request.RiderIdentifier);
                throw new RpcException(new Status(StatusCode.Internal, "Error validating rider"));
            }
        }

        /// <summary>
        /// Gets rider information
        /// </summary>
        public override async Task<GetRiderResponse> GetRider(
            GetRiderRequest request,
            ServerCallContext context)
        {
            try
            {
                var rider = await riderRepository.GetByIdentifierAsync(request.RiderIdentifier);

                return rider == null
                    ? throw new RpcException(new Status(StatusCode.NotFound, "Rider not found"))
                    : mapper.Map<GetRiderResponse>(rider);
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting rider. RiderIdentifier: {RiderIdentifier}", request.RiderIdentifier);
                throw new RpcException(new Status(StatusCode.Internal, "Error getting rider"));
            }
        }
    }
}