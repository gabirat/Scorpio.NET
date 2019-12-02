using System.Globalization;

namespace Scorpio.Gamepad.Models
{
    public class AnalogModel
    {
        public float Value { get; set; }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
