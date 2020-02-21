using Matty.Framework.Validation;
using Newtonsoft.Json;
using Scorpio.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace Scorpio.Api.Validation
{
    public class GpsDataValidator : ISensorDataValidator
    {
        public string SensorKey => "gps";

        public void Validate(SensorData sensorData)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<GpsData>(sensorData.Value);
                data.Validate();
            }
            catch (JsonSerializationException ex)
            {
                throw new ValidationException(ex.Message);
            }
            catch (JsonReaderException ex)
            {
                throw new ValidationException(ex.Message);
            }
        }

        /// <summary>
        /// Model class
        /// </summary>
        private class GpsData : ValidatableParamBase<GpsData>
        {
            [Required(ErrorMessage = "Field 'lat' is required")]
            [JsonProperty("lat")]
            public float? Latitude { get; set; }

            [Required(ErrorMessage = "Field 'lon' is required")]
            [JsonProperty("lon")]
            public float? Longitude { get; set; }
        }
    }
}