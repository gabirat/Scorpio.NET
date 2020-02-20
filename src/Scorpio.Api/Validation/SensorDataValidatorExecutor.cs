using System.ComponentModel.DataAnnotations;
using Scorpio.Api.Models;
using System.Linq;

namespace Scorpio.Api.Validation
{
    public class SensorDataValidatorExecutor
    {
        public static void Execute(SensorData sensorData, bool doThrow = false)
        {
            var validators = SensorDataValidatorsFactory.GetValidators(sensorData.SensorKey).ToList();
            if (!validators.Any()) return;

            try
            {
                foreach (var validator in validators)
                {
                    validator.Validate(sensorData);
                }
            }
            catch (ValidationException)
            {
                if (doThrow) throw;
            }
        }
    }
}
