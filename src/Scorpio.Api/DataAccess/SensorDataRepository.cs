using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Scorpio.Api.Models;
using System;
using System.Threading.Tasks;

namespace Scorpio.Api.DataAccess
{
    public class SensorDataRepository : MongoRepository<SensorData>, ISensorDataRepository
    {
        public SensorDataRepository(IOptions<MongoDbConfiguration> options) : base(options)
        {
        }

        public async Task<long> RemoveRange(string sensorKey, DateTime? from, DateTime? to)
        {
            var dateFrom = from ?? new DateTime();
            var dateTo = to ?? DateTime.UtcNow;

            var builder = Builders<SensorData>.Filter;

            var filter = builder.And(builder.Eq("SensorKey", sensorKey),
                builder.And(
                    builder.Lte("TimeStamp", new BsonDateTime(dateTo)),
                    builder.Gte("TimeStamp", new BsonDateTime(dateFrom))));

            var result = await Collection.DeleteManyAsync(filter);
            return result.DeletedCount;
        }
    }
}
