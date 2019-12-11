using Scorpio.Gamepad.Models;

namespace Scorpio.Gamepad.Processors.Mixing
{

    public class RoverMixer : MixerBase<RoverProcessorResult>
    {
        protected override RoverProcessorResult DoMix(GamepadModel model)
        {
            return new RoverProcessorResult
            {
                Acceleration = (int)model.LeftTrigger,
                Direction = (int)model.LeftTrigger
            };
        }
    }
}
