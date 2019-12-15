using System;
using System.Collections.Generic;
using System.Linq;

namespace Scorpio.GUI
{
    public class CameraConfigModel
    {
        public List<VivotekModel> Vivoteks { get; set; }
        public List<StreamModel> Streams { get; set; }

        public VivotekModel GetVivotekById(string id)
        {
            return Vivoteks.FirstOrDefault(x => x.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase));
        }

        public StreamModel GetStreamById(string id)
        {
            return Streams.FirstOrDefault(x => x.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public class VivotekModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string BaseApiUrl { get; set; }
    }

    public class StreamModel
    {
        public string Id { get; set; }
        public string GstreamerArg { get; set; }
    }
}
