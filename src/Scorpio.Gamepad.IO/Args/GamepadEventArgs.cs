using Scorpio.Gamepad.Models;
using System;

namespace Scorpio.Gamepad.IO.Args
{
    public class GamepadEventArgs : EventArgs
    {
        public GamepadModel Gamepad { get; set; }
    }
}
