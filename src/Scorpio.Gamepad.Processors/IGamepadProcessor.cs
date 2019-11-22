using Scorpio.Gamepad.Models;

namespace Scorpio.Gamepad.Processors
{
    public interface IGamepadProcessor
    {
        ProcessorResult Process(ControllerType type, GamepadModel input);
    }
}
