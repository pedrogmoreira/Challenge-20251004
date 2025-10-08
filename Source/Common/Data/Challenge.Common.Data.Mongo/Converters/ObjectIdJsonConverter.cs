using MongoDB.Bson;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Challenge.Common.Data.Mongo.Converters
{
    /// <summary>
    /// JSON converter for MongoDB ObjectId to enable proper serialization/deserialization
    /// </summary>
    public class ObjectIdJsonConverter : JsonConverter<ObjectId>
    {
        /// <summary>
        /// Reads and converts JSON to ObjectId
        /// </summary>
        /// <param name="reader">The JSON reader</param>
        /// <param name="typeToConvert">The type to convert</param>
        /// <param name="options">Serialization options</param>
        /// <returns>The converted ObjectId</returns>
        public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();
            return new ObjectId(stringValue);
        }

        /// <summary>
        /// Writes ObjectId as JSON string
        /// </summary>
        /// <param name="writer">The JSON writer</param>
        /// <param name="value">The ObjectId to write</param>
        /// <param name="options">Serialization options</param>
        public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
