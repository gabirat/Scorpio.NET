using Scorpio.Gamepad.Models;
using Scorpio.Gamepad.Processors.Mixing;

namespace Scorpio.Gamepad.Processors
{
    public abstract class GamepadProcessorBase<TMixer, TResult> : IGamepadProcessor<TMixer, TResult>
        where TMixer: IMixer<TResult>, new()
        where TResult: class, new()
    {

        public TResult Process(GamepadModel input)
        {
            var mixer = new TMixer();

            ConfigurePipeline(mixer);

            return mixer.Mix(input);
        }

        protected abstract void ConfigurePipeline(TMixer mixer);
    }
}