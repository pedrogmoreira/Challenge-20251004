using System.ComponentModel.DataAnnotations;

namespace Challenge.Microservices.RiderApi.Configuration.Options
{
    /// <summary>
    /// Configuration options for S3 storage
    /// </summary>
    public class S3Options
    {
        /// <summary>
        /// The S3 bucket name where files will be stored
        /// </summary>
        [Required, MinLength(1)]
        public required string BucketName { get; set; }
    }
}
