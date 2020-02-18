using Scorpio.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace Scorpio.Api.Validation
{
    public abstract class SensorDataValidatorBase : ISensorDataValidator
    {
        public virtual string SensorKey { get; }

        public virtual void Validate(SensorData sensorData)
        {
            if (!IsValid(sensorData))
                throw new ValidationException($"Validation for entity {nameof(SensorData)} failed.");
        }

        public abstract bool IsValid(SensorData sensorData);
    }
}
