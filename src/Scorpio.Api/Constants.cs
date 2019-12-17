namespace Scorpio.Api
{
    public static class Constants
    {
        /// <summary>
        /// SignalR topics for messaging/RPC
        /// </summary>
        public static class Topics
        {
            /// <summary>
            /// Represents Ubiquiti SNMP data
            /// </summary>
            public static string Ubiquiti = "ubiquiti";

            /// <summary>
            /// Represents data gathered by on-board sensorics
            /// </summary>
            public static string Sensor = "sensor";
        }
    }
}
