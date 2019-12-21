using System;
using Scorpio.Gamepad.Models;

namespace Scorpio.Gamepad.Processors.Mixing
{
    /// <summary>
    /// Performs exponential lookup for both triggers using function:
    /// y = A + B * exp(c * x)
    /// </summary>
    public class ExponentialCurveOnTriggersFilter : IGamepadFilter
    {
        private static short[] _lookupTable;
        private const short OutputRange = 255;

        /// <summary>
        /// Range 1 - 99, where 1 means more exponential curve, and 99 is more likely linear
        /// </summary>
        private const short CoefficientB = 20;
        
        public GamepadModel Filter(GamepadModel input)
        {
            input.LeftTrigger = PerformLookup(input.LeftTrigger);
            input.RightTrigger = PerformLookup(input.RightTrigger);
            return input;
        }

        public static byte PerformLookup(byte value)
        {
            if (_lookupTable is null) CalculateLookups();

            // Can safely cast to byte, cause of the constrain of 255
            return (byte) ScalingUtils.ConstrainNonnegative(_lookupTable[value], 255);
        }

        private static void CalculateLookups()
        {
            var lut = new short[OutputRange + 1];

            const double coefficientA = -CoefficientB;
            var coefficientC = Math.Log((OutputRange - coefficientA) / CoefficientB) / OutputRange;

            for (var i = 0; i < OutputRange + 1; i++)
            {
                lut[i] = (short)Math.Ceiling(coefficientA + CoefficientB * Math.Exp(coefficientC * i));
                lut[i] = ScalingUtils.ConstrainNonnegative(lut[i], OutputRange);
            }

            _lookupTable = lut;
        }
    }
}
