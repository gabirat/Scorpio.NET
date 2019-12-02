using System.Collections.Generic;
using Scorpio.Gamepad.Models;

namespace Scorpio.Gamepad.Processors.Mixing
{
    public class Mixer
    {
        private readonly List<IFilteringStrategy> _filteringStrategies;

        public Mixer()
        {
            _filteringStrategies = new List<IFilteringStrategy>();
        }

        public void AddFilteringStrategy(IFilteringStrategy strategy)
        {
            _filteringStrategies.Add(strategy);
        }

        public ProcessorResult Mix(GamepadModel model)
        {
            GamepadModel filteringResult = model;

            foreach (var strategy in _filteringStrategies)
            {
                filteringResult = strategy.Filter(filteringResult);
            }

            return DoMix(filteringResult);
        }

        //TODO
        protected virtual ProcessorResult DoMix(GamepadModel model)
        {
            return new ProcessorResult
            {
                FrontLeftSpeed = (int) model.LeftTrigger.Value,
                RearRightSpeed = (int) model.LeftTrigger.Value
            };
        }
    }
}
