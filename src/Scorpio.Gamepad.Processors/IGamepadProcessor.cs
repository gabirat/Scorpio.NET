using Scorpio.Gamepad.Models;
using Scorpio.Gamepad.Processors.Mixing;

namespace Scorpio.Gamepad.Processors
{
    public interface IGamepadProcessor<TMixer, TResult>
        where TMixer : IMixer<TResult>, new()
        where TResult : class, new()
    {
        TResult Process(GamepadModel input);
    }
}
