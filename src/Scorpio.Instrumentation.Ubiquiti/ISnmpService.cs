using System.Collections.Generic;
using SnmpSharpNet;

namespace Scorpio.Instrumentation.Ubiquiti
{
    public interface ISnmpService
    {
        Dictionary<Oid, AsnType> Walk(SnmpVersion version, string rootOid);
    }
}