using Scorpio.Gamepad.Models;

namespace Scorpio.Gamepad.Processors.Mixing
{
    public interface IGamepadFilter
    {
        GamepadModel Filter(GamepadModel input);
    }
}