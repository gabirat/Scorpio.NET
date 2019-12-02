using Scorpio.Gamepad.Models;

namespace Scorpio.Gamepad.Processors.Mixing
{
    public interface IFilteringStrategy
    {
        GamepadModel Filter(GamepadModel input);
    }
}