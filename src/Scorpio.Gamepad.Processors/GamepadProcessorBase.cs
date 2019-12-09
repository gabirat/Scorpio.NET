using Scorpio.Gamepad.Models;
using Scorpio.Gamepad.Processors.Mixing;

namespace Scorpio.Gamepad.Processors
{
    public abstract class GamepadProcessorBase : IGamepadProcessor
    {
        public ProcessorResult Process(ControllerType type, GamepadModel input)
        {
            var mixer = new Mixer();

            ConfigurePipeline(mixer);

            // TODO include type somewhere
            return mixer.Mix(input);
        }

        protected abstract void ConfigurePipeline(Mixer mixer);
    }
}