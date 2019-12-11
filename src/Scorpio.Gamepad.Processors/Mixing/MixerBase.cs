using Scorpio.Gamepad.Models;
using System.Collections.Generic;

namespace Scorpio.Gamepad.Processors.Mixing
{
    public abstract class MixerBase<TResult> : IMixer<TResult> where TResult : class, new()
    {
        protected readonly List<IFilteringStrategy> filteringStrategies;

        public MixerBase()
        {
            filteringStrategies = new List<IFilteringStrategy>();
        }

        public void AddFilteringStrategy(IFilteringStrategy strategy)
        {
            filteringStrategies.Add(strategy);
        }

        public virtual TResult Mix(GamepadModel model)
        {
            GamepadModel filteringResult = model;

            foreach (var strategy in filteringStrategies)
            {
                filteringResult = strategy.Filter(filteringResult);
            }

            return DoMix(filteringResult);
        }

        protected abstract TResult DoMix(GamepadModel model);
    }
}
