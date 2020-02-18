using Scorpio.Api.Models;
using System.Linq;

namespace Scorpio.Api.Validation
{
    public class SensorDataValidatorExecutor
    {
        public static bool Execute(SensorData sensorData, bool doThrow = false)
        {
            var result = true;

            var validators = SensorDataValidatorsFactory.GetValidators(sensorData.SensorKey).ToList();
            if (!validators.Any()) return true;

            if (doThrow)
            {
                foreach (var validator in validators)
                {
                    validator.Validate(sensorData);
                }
            }

            foreach (var validator in validators)
            {
                result &= validator.IsValid(sensorData);
            }

            return result;
        }
    }
}
