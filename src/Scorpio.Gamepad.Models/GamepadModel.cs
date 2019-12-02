namespace Scorpio.Gamepad.Models
{
    public class GamepadModel
    {
        public ThumbstickModel LeftThumbstick { get; set; }
        public ThumbstickModel RightThumbstick { get; set; }
        public AnalogModel LeftTrigger { get; set; }
        public AnalogModel RightTrigger { get; set; }
    }
}
