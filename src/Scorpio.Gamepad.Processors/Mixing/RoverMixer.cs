using Scorpio.Gamepad.Models;

namespace Scorpio.Gamepad.Processors.Mixing
{

    public class RoverMixer : MixerBase<RoverProcessorResult>
    {
        /// <summary>
        /// Produces acc & dir output:
        /// Rover moving forward-backward: dir0, acc ranging -100:100
        /// Rover moving & rotating: dir ranging -1:1, acc ranging -100:100
        /// Rover rotating in spot to the left: acc -200, acc ranging -100:100
        /// Rover rotating in spot to the right: acc 200, acc ranging -100:100
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected override RoverProcessorResult DoMix(GamepadModel model)
        {
            if (model.IsLeftTriggerButtonPressed || model.IsRightTriggerButtonPressed)
            {
                return Rotate(model);
            }

            return ForwardReverse(model);
        }

        private static RoverProcessorResult ForwardReverse(GamepadModel model)
        {
            var result = new RoverProcessorResult
            {
                Acceleration = GetDeltaTriggers(model),
                Direction = GetDirection(model)
            };

            return result;
        }

        private static RoverProcessorResult Rotate(GamepadModel model)
        {
            var result = new RoverProcessorResult
            {
                Acceleration = GetDeltaTriggers(model),
                Direction = GetRotateInSpotDirection(model)
            };

            return result;
        }

        private static float GetDirection(GamepadModel model)
        {
            const float deadZone = 0.2f;

            var leftRightStick = ScalingUtils.ShortToFloat(model.LeftThumbstick.Horizontal);

            if (leftRightStick <= deadZone && leftRightStick >= -deadZone)
                leftRightStick = 0;


            return ScalingUtils.SymmetricalConstrain(leftRightStick, 1.0f);
        }

        private static float GetDeltaTriggers(GamepadModel model)
        {
            var lTrigger = ScalingUtils.ByteToFloat(model.LeftTrigger);
            var rTrigger = ScalingUtils.ByteToFloat(model.RightTrigger);
            var deltaTriggers = rTrigger - lTrigger;
            return ScalingUtils.SymmetricalConstrain(deltaTriggers, 1.0f) * 100.0f;
        }

        private static float GetRotateInSpotDirection(GamepadModel model)
        {
            if (model.IsLeftTriggerButtonPressed) return -200.0f;
            return 200.0f;
        }
    }
}
