using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Scorpio.Api.Models
{
    public class EntityBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
