using Scorpio.Gamepad.Processors.Mixing;

namespace Scorpio.Gamepad.Processors
{
    public class ExponentialGamepadProcessor<TMixer, TResult> : GamepadProcessorBase<TMixer, TResult>
        where TMixer: class, IMixer<TResult>, new()
        where TResult: class, new()
    {

        protected override void ConfigurePipeline(TMixer mixer)
        {
            mixer.AddFilteringStrategy(new ExponentialCurveOnTriggersFilter());
        }
    }
}
