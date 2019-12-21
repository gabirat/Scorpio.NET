namespace Scorpio.Gamepad.Processors
{
    public class RoverProcessorResult
    { 
        public float Acceleration { get; set; }
        public float Direction { get; set; }
        public bool DoRotation { get; set; } = false;
        public float Rotation { get; set; }
    }
}
