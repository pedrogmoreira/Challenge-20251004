using Challenge.Microservices.SubscriptionApi.Infra.Services.Response;

namespace Challenge.Microservices.SubscriptionApi.Infra.Services
{
    /// <summary>
    /// Service for validating rider eligibility
    /// </summary>
    public interface IRiderValidationService
    {
        /// <summary>
        /// Checks if a rider has a category A driver's license (required for motorbike rentals)
        /// </summary>
        /// <param name="riderIdentifier">The unique identifier of the rider</param>
        /// <returns>True if the rider has category A license, false otherwise</returns>
        Task<HasValidVicenseResponse> HasValidLicenseAsync(string riderIdentifier);
    }
}