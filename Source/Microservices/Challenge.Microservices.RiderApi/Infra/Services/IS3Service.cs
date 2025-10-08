namespace Challenge.Microservices.RiderApi.Infra.Services
{
    /// <summary>
    /// Service for managing S3 storage operations
    /// </summary>
    public interface IS3Service
    {
        /// <summary>
        /// Uploads a CNH (driver's license) image to S3 storage
        /// </summary>
        /// <param name="base64Image">Base64 encoded image with data URI (e.g., data:image/png;base64,...)</param>
        /// <param name="userId">The unique identifier of the user</param>
        /// <param name="cnhNumber">The CNH (driver's license) number</param>
        /// <returns>The S3 object key of the uploaded image</returns>
        Task<string> UploadCnhImageAsync(string base64Image, string userId, string cnhNumber);
    }
}
