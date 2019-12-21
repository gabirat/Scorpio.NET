using Scorpio.Gamepad.Models;
using System.Collections.Generic;

namespace Scorpio.Gamepad.Processors.Mixing
{
    public abstract class MixerBase<TResult> : IMixer<TResult> where TResult : class, new()
    {
        protected readonly List<IGamepadFilter> FilteringStrategies;

        protected MixerBase()
        {
            FilteringStrategies = new List<IGamepadFilter>();
        }

        public void AddFilter(IGamepadFilter strategy)
        {
            FilteringStrategies.Add(strategy);
        }

        public virtual TResult Mix(GamepadModel model)
        {
            var filteringResult = model;

            foreach (var strategy in FilteringStrategies)
            {
                filteringResult = strategy.Filter(filteringResult);
            }

            return DoMix(filteringResult);
        }

        protected abstract TResult DoMix(GamepadModel model);
    }
}
