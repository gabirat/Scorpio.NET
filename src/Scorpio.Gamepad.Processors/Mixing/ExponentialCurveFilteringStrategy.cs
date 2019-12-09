using Scorpio.Gamepad.Models;

namespace Scorpio.Gamepad.Processors.Mixing
{
    public class ExponentialCurveFilteringStrategy : IFilteringStrategy
    {
        public GamepadModel Filter(GamepadModel input)
        {
            return input;
        }
    }
}
