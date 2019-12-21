using J2i.Net.XInputWrapper;
using Scorpio.Gamepad.IO.Args;
using Scorpio.Gamepad.Models;
using System;

namespace Scorpio.Gamepad.IO
{
    public interface IGamepadPoller : IDisposable
    {
        event EventHandler<GamepadEventArgs> GamepadStateChanged;
        GamepadModel GetState();
        bool IsConnected { get; }
        void StartPolling();
        void StopPolling();
        void Vibrate();
    }

    public class GamepadPoller : IGamepadPoller
    {
        public event EventHandler<GamepadEventArgs> GamepadStateChanged;
        public bool IsConnected => _controller?.IsConnected ?? false;

        private readonly XboxController _controller;
        private GamepadModel _gamepadState;

        public GamepadPoller(int controllerIndex) : this(controllerIndex, 50) { }

        public GamepadPoller(int controllerIndex, int updateFrequency)
        {
            _controller = XboxController.RetrieveController(controllerIndex);
            XboxController.UpdateFrequency = updateFrequency;
        }

        public void StartPolling()
        {
            _controller.StateChanged += StateChanged;
            XboxController.StartPolling();
        }

        public void StopPolling()
        {
            _controller.StateChanged -= StateChanged;
            XboxController.StopPolling();
        }

        public void Vibrate()
        {
            _controller?.Vibrate(1.0, 1.0, TimeSpan.FromSeconds(0.8));
        }

        public GamepadModel GetState() => _gamepadState;

        private void StateChanged(object sender, XboxControllerStateChangedEventArgs e)
        {
            _gamepadState = Map(e.CurrentInputState);

            var args = new GamepadEventArgs { Gamepad = _gamepadState };

            // Fire event
            GamepadStateChanged?.Invoke(this, args);
        }

        private static GamepadModel Map(XInputState state)
        {
            return new GamepadModel
            {
                LeftThumbstick = new ThumbstickModel
                {
                    Vertical = state.Gamepad.sThumbLY,
                    Horizontal = state.Gamepad.sThumbLX
                },
                RightThumbstick = new ThumbstickModel
                {
                    Horizontal = state.Gamepad.sThumbRX,
                    Vertical = state.Gamepad.sThumbRY
                },
                LeftTrigger = state.Gamepad.bLeftTrigger,
                RightTrigger = state.Gamepad.bRightTrigger,
                IsLeftTriggerButtonPressed = state.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_LEFT_SHOULDER),
                IsRightTriggerButtonPressed = state.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_RIGHT_SHOULDER)
            };
        }

        public void Dispose()
        {
            XboxController.StopPolling();
        }
    }
}
