using Scorpio.Gamepad.Models;

namespace Scorpio.Gamepad.Processors.Mixing
{
    public interface IMixer<TResult> where TResult: class, new()
    {
        TResult Mix(GamepadModel model);
        void AddFilteringStrategy(IFilteringStrategy strategy);
    }
}
