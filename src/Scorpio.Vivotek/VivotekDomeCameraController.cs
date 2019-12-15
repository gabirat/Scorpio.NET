using Microsoft.Extensions.Logging;
using Scorpio.Vivotek.DomeCamera;
using System.Threading.Tasks;
using System;

namespace Scorpio.Vivotek
{
    public class VivotekDomeCameraController : HttpClientBase
    {
        public string ApiUrl { get; set; }

        public VivotekDomeCameraController(ILogger<VivotekDomeCameraController> logger) : base(logger)
        {
        }

        public async Task Control(CameraCommand command)
        {
            var queryParam = CommandsDictionary.Commands[command];
            var endpoint = ApiUrl + queryParam;
            SetBasicAuthHeaders();
            Logger.LogInformation($"Sending camera command: {command}, constructed url: {endpoint}");
            await GetRawAsync(endpoint);
        }

        public async Task SetSpeed(CameraSpeedCommand command, sbyte speed)
        {
            if (speed > 5 || speed < -5)
                throw new ArgumentException($"Speed value should be between -5 and 5");

            var queryParam = CommandsDictionary.SpeedCommands[command];
            var endpoint = ApiUrl + queryParam + speed;
            SetBasicAuthHeaders();
            Logger.LogInformation($"Sending camera command: {command}, constructed url: {endpoint}");
            await GetRawAsync(endpoint);
        }
    }
}
