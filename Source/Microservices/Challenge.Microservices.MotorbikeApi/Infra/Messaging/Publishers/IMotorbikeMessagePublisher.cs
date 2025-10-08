using Challenge.Microservices.MotorbikeApi.Infra.Data.Entities;

namespace Challenge.Microservices.MotorbikeApi.Infra.Messaging.Publishers
{
    /// <summary>
    /// Message publisher interface for motorbike events
    /// </summary>
    public interface IMotorbikeMessagePublisher
    {
        Task PublishMotorbikeRegisteredAsync(Motorbike motorbike);
    }
}