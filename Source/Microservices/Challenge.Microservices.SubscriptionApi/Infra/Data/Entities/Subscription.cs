using Challenge.Common.Data.Mongo.Entities;

namespace Challenge.Microservices.SubscriptionApi.Infra.Data.Entities
{
    /// <summary>
    /// Represents a rental subscription for a motorbike
    /// </summary>
    public class Subscription : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the motorbike
        /// </summary>
        public required string Identifier { get; set; }

        /// <summary>
        /// The Identifier of the rider renting the motorbike
        /// </summary>
        public required string RiderIdentifier { get; set; }

        /// <summary>
        /// The Identifier of the motorbike being rented
        /// </summary>
        public required string MotorbikeIdentifier { get; set; }

        /// <summary>
        /// The rental plan (7, 15, 30, 45, or 50 days)
        /// </summary>
        public required int PlanDays { get; set; }

        /// <summary>
        /// Daily cost based on the plan
        /// </summary>
        public required decimal DailyCost { get; set; }

        /// <summary>
        /// Start date of the rental (first day after creation)
        /// </summary>
        public required DateTime StartDate { get; set; }

        /// <summary>
        /// Expected end date of the rental
        /// </summary>
        public required DateTime ExpectedEndDate { get; set; }

        /// <summary>
        /// Predicted end date (initially same as ExpectedEndDate)
        /// </summary>
        public required DateTime PredictedEndDate { get; set; }

        /// <summary>
        /// Actual return date (null until returned)
        /// </summary>
        public DateTime? ActualReturnDate { get; set; }

        /// <summary>
        /// Total cost of the rental
        /// </summary>
        public decimal? TotalCost { get; set; }

        /// <summary>
        /// Status of the subscription
        /// </summary>
        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    }
}