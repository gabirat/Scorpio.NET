using Microsoft.Extensions.Configuration;
using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scorpio.Instrumentation.Ubiquiti
{
    public class UbiquitiStatsProvider
    {
        public virtual string RootOip { get; set; } = "1.3.6.1.4.1.41112.1";

        public virtual Dictionary<string, PhysicalProperty> ResponseFilter { get; set; } = new Dictionary<string, PhysicalProperty>
        {
            { "1.3.6.1.4.1.41112.1.4.1.1.6.1", new PhysicalProperty { Magnitude = "signalPower", Unit = "dBm" }},
            { "1.3.6.1.4.1.41112.1.4.1.1.4.1", new PhysicalProperty { Magnitude = "frequency", Unit = "MHz" } },
        };

        private readonly ISnmpService _snmpService;

        // Constructor for autofac
        public UbiquitiStatsProvider(IConfiguration config)
        {
            var agentIp = config["Ubiquiti:SnmpAgentIp"];
            var community = config["Ubiquiti:SnmpCommunity"];
            _snmpService = new SnmpAdapter(agentIp, community);
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
                var response = _snmpService.Walk(SnmpVersion.Ver1, RootOip);

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
