namespace Scorpio.Vivotek.DomeCamera
{
    public enum CameraCommand : byte
    {
        ZoomIn = 0,
        ZoomOut = 2,
        FocusNear = 4,
        FocusFar = 6,
        Autofocus = 8,
        Left = 16,
        Right = 18,
        Up = 20,
        Down = 22,
        Home = 24
    }

    public enum CameraSpeedCommand : byte
    {
        ZoomSpeed = 0,
        PanSpeed = 2,
        TiltSpeed = 4
    }
}
