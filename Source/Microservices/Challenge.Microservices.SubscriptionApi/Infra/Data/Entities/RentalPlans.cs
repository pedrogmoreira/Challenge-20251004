namespace Challenge.Microservices.SubscriptionApi.Infra.Data.Entities
{
    /// <summary>
    /// Available rental plans with pricing
    /// </summary>
    public static class RentalPlans
    {
        public static readonly Dictionary<int, decimal> Plans = new()
        {
            {  7, 30.00m },  // R$ 30/day
            { 15, 28.00m },  // R$ 28/day
            { 30, 22.00m },  // R$ 22/day
            { 45, 20.00m },  // R$ 20/day
            { 50, 18.00m }   // R$ 18/day
        };

        public static readonly Dictionary<int, decimal> EarlyReturnPenalties = new()
        {
            {  7, 0.20m },  // 20% penalty
            { 15, 0.40m }   // 40% penalty
        };

        public const decimal LateDailyCost = 50.00m;
    }
}