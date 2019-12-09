namespace Scorpio.Gamepad.Models
{
    public class GamepadModel
    {
        public ThumbstickModel LeftThumbstick { get; set; }
        public ThumbstickModel RightThumbstick { get; set; }
        public byte LeftTrigger { get; set; }
        public byte RightTrigger { get; set; }
    }
}
