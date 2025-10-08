using Challenge.Common.Data.Mongo.Interfaces;
using Challenge.Microservices.MotorbikeApi.Infra.Data.Entities;

namespace Challenge.Microservices.MotorbikeApi.Infra.Data.Repositories
{
    /// <summary>
    /// Repository interface for MotorbikeNotification operations
    /// </summary>
    public interface IMotorbikeNotificationRepository : IMongoRepository<MotorbikeNotification>
    {
    }
}