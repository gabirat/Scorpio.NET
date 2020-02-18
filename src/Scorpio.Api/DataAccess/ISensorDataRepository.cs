using System;
using System.Threading.Tasks;
using Scorpio.Api.Models;

namespace Scorpio.Api.DataAccess
{
    public interface ISensorDataRepository : IGenericRepository<SensorData, string>
    {
        /// <summary>
        /// Delete Sensor Entries by filters
        /// </summary>
        /// <param name="sensorKey"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>Deleted count</returns>
        Task<long> RemoveRange(string sensorKey, DateTime? from, DateTime? to);
    }
}
