using Challenge.Microservices.SubscriptionApi.Infra.Data.Entities;
using Challenge.Microservices.SubscriptionApi.Infra.Services.Response;

namespace Challenge.Microservices.SubscriptionApi.Infra.Services
{
    /// <summary>
    /// Service for calculating subscription costs
    /// </summary>
    public interface ISubscriptionCostService
    {
        /// <summary>
        /// Calculates the total rental cost with breakdown of daily rates, early return penalties (20-40%), and late fees (R$50/day)
        /// </summary>
        /// <param name="subscription">The rental subscription with plan type, start date, and expected end date</param>
        /// <param name="returnDate">The actual or planned return date for cost calculation</param>
        /// <returns>Detailed cost breakdown including total, daily charges, penalties, and additional fees</returns>
        SubscriptionCostResponse CalculateCost(Subscription subscription, DateTime returnDate);
    }
}