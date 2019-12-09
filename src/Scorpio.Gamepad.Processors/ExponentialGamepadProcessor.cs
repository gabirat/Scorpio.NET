using Scorpio.Gamepad.Processors.Mixing;

namespace Scorpio.Gamepad.Processors
{
    public class ExponentialGamepadProcessor : GamepadProcessorBase<RoverMixer, RoverProcessorResult>
    {
        protected override void ConfigurePipeline(RoverMixer mixer)
        {
            mixer.AddFilteringStrategy(new ExponentialCurveFilteringStrategy());
        }
    }
}
