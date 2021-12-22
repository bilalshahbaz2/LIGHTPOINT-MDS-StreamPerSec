using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Services
{
    public interface IFieldMapper
    {
        List<string> GetFields();
        Dictionary<string, string> Map(Dictionary<string, string> FactsetFields);
        ResponseBag ToSourceCollector(List<Dictionary<string, string>> FactsetFields, IncomingRequest request);
        ResponseBag ToSourceCollector(Dictionary<string, string> FactsetFields, IncomingRequest request);

    }
}
