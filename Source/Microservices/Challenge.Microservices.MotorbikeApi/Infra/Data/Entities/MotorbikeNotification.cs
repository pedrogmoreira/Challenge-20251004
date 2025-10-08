using Challenge.Common.Data.Mongo.Entities;
//using MongoDB.Driver;

namespace Challenge.Microservices.MotorbikeApi.Infra.Data.Entities
{
    /// <summary>
    /// Represents a notification for 2024 year motorbike registrations
    /// </summary>
    public class MotorbikeNotification : BaseEntity
    {
        /// <summary>
        /// The ID of the motorbike that triggered the notification
        /// </summary>
        public required string MotorbikeIdentifier { get; set; }

        /// <summary>
        /// The year of the motorbike (should be 2024)
        /// </summary>
        public required int Year { get; set; }

        /// <summary>
        /// The model of the motorbike
        /// </summary>
        public required string Model { get; set; }

        /// <summary>
        /// The license plate of the motorbike
        /// </summary>
        public required string LicensePlate { get; set; }

        /// <summary>
        /// The date when the notification was created
        /// </summary>
        public DateTime NotificationDate { get; set; }
    }
}