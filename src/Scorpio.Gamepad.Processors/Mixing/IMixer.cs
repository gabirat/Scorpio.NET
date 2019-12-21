using Scorpio.Gamepad.Models;

namespace Scorpio.Gamepad.Processors.Mixing
{
    public interface IMixer<out TResult> where TResult: class, new()
    {
        TResult Mix(GamepadModel model);
        void AddFilter(IGamepadFilter strategy);
    }
}
