using System.Collections.Generic;
using System.Net;
using System;
using System.Linq;

namespace Scorpio.Instrumentation.Ubiquiti
{
    public class UbiquitiPollerConfiguration
    {
        public bool EnablePoller { get; set; }
        public string SnmpAgentIp { get; set; }
        public string SnmpCommunity { get; set; }
        public string RootOid { get; set; }
        public List<OidConfigModel> Oids { get; set; }

        public void EnsureValidConfig()
        {
            if (!IPAddress.TryParse(SnmpAgentIp, out var address))
                throw new InvalidOperationException($"Invalid param: {nameof(SnmpAgentIp)}");

            if (string.IsNullOrWhiteSpace(SnmpCommunity))
                throw new InvalidOperationException($"Invalid param: {nameof(SnmpCommunity)}");

            if (string.IsNullOrWhiteSpace(RootOid))
                throw new InvalidOperationException($"Invalid param: {nameof(RootOid)}");

            if (Oids is null)
                throw new InvalidOperationException("Invalid param: no oids specified");

            var magnitudes = Oids.Select(x => x.PhysicalProperty.Magnitude).ToList();
            if (magnitudes.Count != magnitudes.Distinct().Count())
                throw new InvalidOperationException("Specified oid list is invalid: magnitudes should be unique");
        }

        public class OidConfigModel
        {
            public string Oid { get; set; }
            public PhysicalProperty PhysicalProperty { get; set; }
        }
    }
}
