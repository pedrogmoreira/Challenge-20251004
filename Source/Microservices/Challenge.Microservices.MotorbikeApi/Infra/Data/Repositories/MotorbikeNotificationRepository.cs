using Challenge.Common.Data.Mongo.Repositories;
using Challenge.Microservices.MotorbikeApi.Infra.Data.Entities;
using MongoDB.Driver;

namespace Challenge.Microservices.MotorbikeApi.Infra.Data.Repositories
{
    /// <summary>
    /// Repository implementation for MotorbikeNotification
    /// </summary>
    public class MotorbikeNotificationRepository (IMongoDatabase database) 
        : MongoRepository<MotorbikeNotification>(database, "MotorbikeNotifications"), IMotorbikeNotificationRepository
    {
    }
}