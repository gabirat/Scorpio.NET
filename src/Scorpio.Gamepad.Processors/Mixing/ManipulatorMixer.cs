using Scorpio.Gamepad.Models;

namespace Scorpio.Gamepad.Processors.Mixing
{
    public class ManipulatorMixer : MixerBase<ManipulatorProcessorResult>
    {
        protected override ManipulatorProcessorResult DoMix(GamepadModel model)
        {
            return new ManipulatorProcessorResult();
        }
    }
}
