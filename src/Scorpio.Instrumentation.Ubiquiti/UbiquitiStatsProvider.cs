using Microsoft.Extensions.Options;
using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scorpio.Instrumentation.Ubiquiti
{
    public class UbiquitiStatsProvider
    {
        private readonly string _rootOip;
        private readonly Dictionary<string, PhysicalProperty> _responseFilterOids;
        private readonly ISnmpService _snmpService;

        // Constructor for autofac
        public UbiquitiStatsProvider(IOptions<UbiquitiPollerConfiguration> config)
        {
            config.Value.EnsureValidConfig();
            _responseFilterOids = config.Value.Oids.ToDictionary(x => x.Oid, x => x.PhysicalProperty);

            _rootOip = config.Value.RootOid;
            _snmpService = new SnmpAdapter(config.Value.SnmpAgentIp, config.Value.SnmpCommunity);
        }

        // Constructor for unit testing purposes
        public UbiquitiStatsProvider(ISnmpService snmp, Dictionary<string, PhysicalProperty> responseFilter)
        {
            _snmpService = snmp;
            _responseFilterOids = responseFilter;
        }

        public Task<Dictionary<string, string>> GetStatsAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                var response = _snmpService.Walk(SnmpVersion.Ver1, _rootOip);

                return ProcessResponse(response);
            }, cancellationToken);
        }

        public Dictionary<string, string> ProcessResponse(Dictionary<Oid, AsnType> response)
        {
            if (response is null) 
                return new Dictionary<string, string>();

            var filtered = response
                .Where(r => _responseFilterOids.Keys.Contains(r.Key.ToString()))
                .ToDictionary(x => x.Key.ToString(), x => x.Value.ToString());

            return filtered.ToDictionary(x => _responseFilterOids[x.Key].Magnitude, FormatUnit(_responseFilterOids));
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
