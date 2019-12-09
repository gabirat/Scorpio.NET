using Scorpio.Gamepad.Processors.Mixing;

namespace Scorpio.Gamepad.Processors
{
    public class ExponentialGamepadProcessor : GamepadProcessorBase
    {
        protected override void ConfigurePipeline(Mixer mixer)
        {
            mixer.AddFilteringStrategy(new ExponentialCurveFilteringStrategy());
        }
    }
}
