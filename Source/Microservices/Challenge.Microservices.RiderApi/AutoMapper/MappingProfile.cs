using AutoMapper;
using Challenge.Common.Messaging.Grpc;
using Challenge.Microservices.RiderApi.Commands;
using Challenge.Microservices.RiderApi.Infra.Data.Entities;

namespace Challenge.Microservices.RiderApi.AutoMapper
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
            CreateMap<RegisterRiderCommand, Rider>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Rider, GetRiderResponse>();
        }
    }
}