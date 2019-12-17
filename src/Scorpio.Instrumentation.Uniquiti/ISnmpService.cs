using System.Collections.Generic;
using SnmpSharpNet;

namespace Scorpio.Instrumentation.Uniquiti
{
    public interface ISnmpService
    {
        Dictionary<Oid, AsnType> Walk(SnmpVersion version, string rootOid);
    }
}