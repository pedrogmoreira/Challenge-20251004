using AutoMapper;
using Challenge.Microservices.SubscriptionApi.Commands;
using Challenge.Microservices.SubscriptionApi.Infra.Data.Entities;
using Challenge.Microservices.SubscriptionApi.Queries.Responses;

namespace Challenge.Microservices.SubscriptionApi.AutoMapper
{
    /// <summary>
    /// AutoMapper profile for mapping between commands and entities
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Configures the mapping rules for the application
        /// </summary>
        public MappingProfile()
        {
            CreateMap<CreateSubscriptionCommand, Subscription>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.MapFrom(_ => DateTime.UtcNow))
            .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow))
            .ForMember(d => d.Status, o => o.MapFrom(_ => SubscriptionStatus.Active))
            .ForMember(d => d.DailyCost, o => o.MapFrom(src => RentalPlans.Plans[src.PlanDays]));

            CreateMap<Subscription, SubscriptionResponse>();
        }
    }
}