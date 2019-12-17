using System;
using SnmpSharpNet;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scorpio.Instrumentation.Uniquiti
{
    public class UbiquitiStatsProvider
    {
        public virtual string SnmpAgentIp { get; set; } = "10.0.10.255";
        public virtual string SnmpCommunity { get; set; } = "scorpio";
        public virtual string RootOip { get; set; } = "1.3.6.1.4.1.41112.1";

        public virtual Dictionary<string, PhysicalProperty> ResponseFilter { get; set; } = new Dictionary<string, PhysicalProperty>
        {
            { "1.3.6.1.4.1.41112.1.4.1.1.6.1", new PhysicalProperty { Magnitude = "signalPower", Unit = "dBm" }},
            { "1.3.6.1.4.1.41112.1.4.1.1.4.1", new PhysicalProperty { Magnitude = "frequency", Unit = "MHz" } },
        };

        private readonly ISnmpService _snmpService;

        public UbiquitiStatsProvider()
        {
            _snmpService = new SnmpAdapter(SnmpAgentIp, SnmpCommunity);
        }

        // Constructor for unit testing purposes
        public UbiquitiStatsProvider(ISnmpService snmp)
        {
            _snmpService = snmp;
        }

        public Task<Dictionary<string, string>> GetStatsAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                Dictionary<Oid, AsnType> response = _snmpService.Walk(SnmpVersion.Ver1, RootOip);

                return ProcessResponse(response);
            });
        }

        public Dictionary<string, string> ProcessResponse(Dictionary<Oid, AsnType> response)
        {
            var filtered = response
                .Where(r => ResponseFilter.Keys.Contains(r.Key.ToString()))
                .ToDictionary(x => x.Key.ToString(), x => x.Value.ToString());

            return filtered.ToDictionary(x => ResponseFilter[x.Key].Magnitude, FormatUnit(ResponseFilter));
        }

        private static Func<KeyValuePair<string, string>, string> FormatUnit(Dictionary<string, PhysicalProperty> filterDict)
        {
            return x => $"{x.Value} [{filterDict[x.Key].Unit}]";
        }
    }


    // Adapter for unit testing
    public class SnmpAdapter : SimpleSnmp, ISnmpService
    {
        public SnmpAdapter(string peerName, string community) : base(peerName, community)
        {
        }
    }
}
