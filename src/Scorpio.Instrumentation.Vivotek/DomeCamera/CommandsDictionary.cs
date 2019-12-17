using System.Collections.Generic;

namespace Scorpio.Instrumentation.Vivotek.DomeCamera
{
    public static class CommandsDictionary
    {
        public static Dictionary<CameraCommand, string> Commands { get; } = new Dictionary<CameraCommand, string>
            {
                { CameraCommand.Left, "move=left" },
                { CameraCommand.Right, "move=right" },
                { CameraCommand.Up, "move=up" },
                { CameraCommand.Down, "move=down" },
                { CameraCommand.Home, "move=home" },
                { CameraCommand.ZoomIn, "zoom=tele" },
                { CameraCommand.ZoomOut, "zoom=wide" },
                { CameraCommand.FocusFar, "focus=far" },
                { CameraCommand.FocusNear, "focus=near" },
                { CameraCommand.Autofocus, "focus=auto" }
            };

        public static Dictionary<CameraSpeedCommand, string> SpeedCommands { get; } = new Dictionary<CameraSpeedCommand, string>
        {
            { CameraSpeedCommand.ZoomSpeed, "speedzoom=" },
            { CameraSpeedCommand.PanSpeed, "speedpan=" },
            { CameraSpeedCommand.TiltSpeed, "speedtilt=" }
        };
    }
}
