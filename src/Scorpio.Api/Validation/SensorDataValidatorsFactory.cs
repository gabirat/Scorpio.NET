using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Scorpio.Api.Validation
{
    public class SensorDataValidatorsFactory
    {
        public static IEnumerable<ISensorDataValidator> GetValidators(string sensorKey)
        {
            var ret = new List<ISensorDataValidator>();

            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            var validators = types.Where(x => !x.IsAbstract && !x.IsInterface && typeof(ISensorDataValidator).IsAssignableFrom(x)).ToArray();

            foreach (var validator in validators)
            {
                if (!(Activator.CreateInstance(validator) is ISensorDataValidator concreteValidator)) continue;

                if (string.Equals(concreteValidator.SensorKey, sensorKey, StringComparison.InvariantCultureIgnoreCase))
                    ret.Add(concreteValidator);
            }

            return ret;
        }
    }
}