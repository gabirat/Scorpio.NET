using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Scorpio.Api.Models
{
    public class EntityBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
