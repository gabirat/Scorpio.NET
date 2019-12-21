using Scorpio.Gamepad.Models;
using Scorpio.Gamepad.Processors.Mixing;

namespace Scorpio.Gamepad.Processors
{
    public abstract class GamepadProcessorBase<TMixer, TResult> : IGamepadProcessor<TMixer, TResult>
        where TMixer: IMixer<TResult>, new()
        where TResult: class, new()
    {
        private readonly TMixer _mixer;

        protected GamepadProcessorBase()
        {
            _mixer = new TMixer();

            // ReSharper disable once VirtualMemberCallInConstructor
            ConfigurePipeline(_mixer);
        }

        public TResult Process(GamepadModel input)
        {
            return _mixer.Mix(input);
        }

        protected abstract void ConfigurePipeline(TMixer mixer);
    }
}