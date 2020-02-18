using Newtonsoft.Json.Linq;
using Scorpio.Api.Models;
using System;

namespace Scorpio.Api.Validation
{
    public class GpsDataValidator : SensorDataValidatorBase
    {
        public override string SensorKey => "gps";

        public override bool IsValid(SensorData sensorData)
        {
            try
            {
                var json = JObject.Parse(sensorData.Value);

                if (json.ContainsKey("lat") && json.ContainsKey("lon"))
                {
                    var lat = json.GetValue("lat");
                    var lon = json.GetValue("lon");

                    if (float.TryParse(lat.ToString(), out _) && float.TryParse(lon.ToString(), out _))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}