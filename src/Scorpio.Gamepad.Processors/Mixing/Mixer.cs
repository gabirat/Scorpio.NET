using System.Collections.Generic;
using Scorpio.Gamepad.Models;

namespace Scorpio.Gamepad.Processors.Mixing
{
    public interface IMixer<TResult> where TResult: class, new()
    {
        TResult Mix(GamepadModel model);
    }

    public class RoverMixer : IMixer<RoverProcessorResult>
    {
        private readonly List<IFilteringStrategy> _filteringStrategies;

        public RoverMixer()
        {
            _filteringStrategies = new List<IFilteringStrategy>();
        }

        public void AddFilteringStrategy(IFilteringStrategy strategy)
        {
            _filteringStrategies.Add(strategy);
        }

        public RoverProcessorResult Mix(GamepadModel model)
        {
            GamepadModel filteringResult = model;

            foreach (var strategy in _filteringStrategies)
            {
                filteringResult = strategy.Filter(filteringResult);
            }

            return DoMix(filteringResult);
        }

        protected virtual RoverProcessorResult DoMix(GamepadModel model)
        {
            return new RoverProcessorResult
            {
                Acceleration = (int) model.LeftTrigger,
                Direction = (int) model.LeftTrigger
            };
        }
    }
}
