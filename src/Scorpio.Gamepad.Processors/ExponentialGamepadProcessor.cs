using Scorpio.Gamepad.Models;
using Scorpio.Gamepad.Processors.Mixing;

namespace Scorpio.Gamepad.Processors
{
    public class ExponentialGamepadProcessor : IGamepadProcessor
    {
        public ProcessorResult Process(ControllerType type, GamepadModel input)
        {
            var mixer = new Mixer();
            mixer.AddFilteringStrategy(new ExponentialCurveFilteringStrategy());

            // TODO include type somewhere
            return mixer.Mix(input);
        }
    }
}
