using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Challenge.Common.Data.Mongo.Entities
{
    /// <summary>
    /// Base entity class for MongoDB documents with common audit fields
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp when the entity was created
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp when the entity was last updated
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; }
    }
}
