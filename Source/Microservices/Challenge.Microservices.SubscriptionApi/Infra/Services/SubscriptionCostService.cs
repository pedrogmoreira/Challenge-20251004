using Challenge.Microservices.SubscriptionApi.Infra.Data.Entities;
using Challenge.Microservices.SubscriptionApi.Infra.Services.Response;

namespace Challenge.Microservices.SubscriptionApi.Infra.Services
{
    /// <summary>
    /// Implementation of subscription cost calculation
    /// </summary>
    public class SubscriptionCostService : ISubscriptionCostService
    {
        public SubscriptionCostResponse CalculateCost(Subscription subscription, DateTime returnDate)
        {
            var response = new SubscriptionCostResponse();
            var daysUsed = (returnDate.Date - subscription.StartDate.Date).Days + 1;

            response.DaysUsed = daysUsed;
            response.DailiesTotal = daysUsed * subscription.DailyCost;

            // Early return - before expected end date
            if (returnDate.Date < subscription.ExpectedEndDate.Date)
            {
                var daysNotUsed = (subscription.ExpectedEndDate.Date - returnDate.Date).Days;
                var unusedCost = daysNotUsed * subscription.DailyCost;

                // Apply penalty based on plan
                if (RentalPlans.EarlyReturnPenalties.TryGetValue(subscription.PlanDays, out var penaltyRate))
                {
                    response.Penalty = unusedCost * penaltyRate;
                }

                response.TotalCost = response.DailiesTotal + response.Penalty;
            }
            // Late return - after expected end date
            else if (returnDate.Date > subscription.ExpectedEndDate.Date)
            {
                response.AdditionalDays = (returnDate.Date - subscription.ExpectedEndDate.Date).Days;
                response.AdditionalCost = response.AdditionalDays * RentalPlans.LateDailyCost;
                response.TotalCost = response.DailiesTotal + response.AdditionalCost;
            }
            // On time return
            else
            {
                response.TotalCost = response.DailiesTotal;
            }

            return response;
        }
    }
}