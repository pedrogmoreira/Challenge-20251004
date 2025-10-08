using Amazon.S3;
using Amazon.S3.Model;
using Challenge.Microservices.RiderApi.Configuration.Options;
using Microsoft.Extensions.Options;

namespace Challenge.Microservices.RiderApi.Infra.Services
{
    /// <summary>
    /// Implementation of S3 storage service for managing file uploads
    /// </summary>
    public class S3Service(
        IAmazonS3 s3Client,
        IOptions<S3Options> s3Options,
        ILogger<S3Service> logger) : IS3Service
    {
        private readonly IAmazonS3 _s3Client = s3Client;
        private readonly string _bucketName = s3Options.Value.BucketName;
        private readonly ILogger<S3Service> _logger = logger;

        /// <summary>
        /// Uploads a CNH image to S3 storage with automatic format detection
        /// </summary>
        /// <param name="base64Image">Base64 encoded image (PNG or BMP format)</param>
        /// <param name="userId">User identifier for organizing files</param>
        /// <param name="cnhNumber">CNH number for file naming</param>
        /// <returns>The S3 key path where the image was stored</returns>
        /// <exception cref="ArgumentException">Thrown when base64 format is invalid</exception>
        /// <exception cref="InvalidOperationException">Thrown when S3 upload fails</exception>
        public async Task<string> UploadCnhImageAsync(string base64Image, string userId, string cnhNumber)
        {
            try
            {
                // Detect content type and extension
                var contentType = DetectContentType(base64Image);
                var extension = GetExtensionFromContentType(contentType);

                var base64Data = base64Image.Contains(',')
                    ? base64Image.Split(',')[1]
                    : base64Image;

                var imageBytes = Convert.FromBase64String(base64Data);
                var key = GenerateKey(userId, cnhNumber, extension);

                using var memoryStream = new MemoryStream(imageBytes);

                var request = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    InputStream = memoryStream,
                    ContentType = contentType,
                    Metadata =
                {
                    ["user-id"] = userId,
                    ["cnh-number"] = cnhNumber,
                    ["upload-date"] = DateTime.UtcNow.ToString("O")
                }
                };

                var response = await _s3Client.PutObjectAsync(request);

                _logger.LogInformation(
                    "CNH image uploaded successfully. UserId: {UserId}, CnhNumber: {CnhNumber}, Key: {Key}, ContentType: {ContentType}",
                    userId, cnhNumber, key, contentType);

                return key;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex,
                    "Error uploading CNH image to S3. UserId: {UserId}, CnhNumber: {CnhNumber}",
                    userId, cnhNumber);
                throw new InvalidOperationException("Failed to upload CNH image", ex);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex,
                    "Invalid base64 format. UserId: {UserId}, CnhNumber: {CnhNumber}",
                    userId, cnhNumber);
                throw new ArgumentException("Invalid base64 image format", ex);
            }
        }

        /// <summary>
        /// Detects the content type from the base64 data URI prefix
        /// </summary>
        /// <param name="base64Image">Base64 string with or without data URI</param>
        /// <returns>The detected content type (e.g., "image/png")</returns>
        private static string DetectContentType(string base64Image)
        {
            // Check if has data URI scheme
            if (base64Image.StartsWith("data:image/"))
            {
                var endIndex = base64Image.IndexOf(';');
                if (endIndex > 0)
                {
                    return base64Image[5..endIndex]; // Extract "image/png", "image/bmp", etc
                }
            }

            // Default to png if not specified
            return "image/png";
        }

        /// <summary>
        /// Gets the file extension based on the content type
        /// </summary>
        /// <param name="contentType">MIME type (e.g., "image/png")</param>
        /// <returns>File extension without dot (e.g., "png")</returns>
        private static string GetExtensionFromContentType(string contentType)
        {
            return contentType.ToLowerInvariant() switch
            {
                "image/png" => "png",
                "image/bmp" => "bmp",
                _ => "png" // Default
            };
        }

        /// <summary>
        /// Generates the S3 object key for storing CNH images
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="cnhNumber">CNH number</param>
        /// <param name="extension">File extension</param>
        /// <returns>S3 key in format: cnh/{userId}/{cnhNumber}.{extension}</returns>
        private static string GenerateKey(string userId, string cnhNumber, string extension)
        {
            // Format: cnh/{userId}/{cnhNumber}.{extension}
            return $"cnh/{userId}/{cnhNumber}.{extension}";
        }
    }
}
