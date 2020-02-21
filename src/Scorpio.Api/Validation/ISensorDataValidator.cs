using Scorpio.Api.Models;

namespace Scorpio.Api.Validation
{
    /// <summary>
    /// Contract for SensorData validation
    /// </summary>
    public interface ISensorDataValidator
    {
        /// <summary>
        /// SensorKey on which validator will be applied
        /// </summary>
        string SensorKey { get; }

        /// <summary>
        /// Throws exception when SensorData is not valid
        /// </summary>
        /// <param name="sensorData"></param>
        void Validate(SensorData sensorData);
    }
}
