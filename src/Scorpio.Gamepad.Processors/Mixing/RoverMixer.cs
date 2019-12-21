using Scorpio.Gamepad.Models;

namespace Scorpio.Gamepad.Processors.Mixing
{

    public class RoverMixer : MixerBase<RoverProcessorResult>
    {
        protected override RoverProcessorResult DoMix(GamepadModel model)
        {
            if (model.IsRightTriggerButtonPressed)
            {
                return Rotate(model);
            }

            return ForwardReverse(model);
        }

        private static RoverProcessorResult ForwardReverse(GamepadModel model)
        {
            var stick = ScalingUtils.ShortToFloat(model.LeftThumbstick.Horizontal);
            var lTrigger = ScalingUtils.ByteToFloat(model.LeftTrigger);
            var rTrigger = ScalingUtils.ByteToFloat(model.RightTrigger);
            var deltaTriggers = rTrigger - lTrigger;
            
            var result = new RoverProcessorResult
            {
                Acceleration = ScalingUtils.SymmetricalConstrain(deltaTriggers, 1.0f),
                Direction = ScalingUtils.SymmetricalConstrain(stick, 1.0f)
            };

            return result;
        }

        private static RoverProcessorResult Rotate(GamepadModel model)
        {
            var stick = ScalingUtils.ShortToFloat(model.LeftThumbstick.Horizontal);

            var result = new RoverProcessorResult
            {
                DoRotation = true,
                Rotation = ScalingUtils.SymmetricalConstrain(stick, 1.0f)
            };

            return result;
        }
    }
}
