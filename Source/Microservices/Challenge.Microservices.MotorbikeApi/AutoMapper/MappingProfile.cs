using AutoMapper;
using Challenge.Microservices.MotorbikeApi.Commands;
using Challenge.Microservices.MotorbikeApi.Infra.Data.Entities;
using Challenge.Microservices.MotorbikeApi.Infra.Messaging.Events;
using Challenge.Microservices.MotorbikeApi.Queries.Responses;

namespace Challenge.Microservices.MotorbikeApi.AutoMapper
{
    /// <summary>
    /// AutoMapper profile for mapping between commands and entities
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterMotorbikeCommand, Motorbike>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsAvailable, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<Motorbike, MotorbikeResponse>();

            CreateMap<Motorbike, MotorbikeRegisteredEvent>()
                .ForMember(dest => dest.MotorbkeIdentifier, opt => opt.MapFrom(src => src.Identifier))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<MotorbikeRegisteredEvent, MotorbikeNotification>()
                .ForMember(dest => dest.NotificationDate, opt => opt.MapFrom(src => src.Timestamp))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}