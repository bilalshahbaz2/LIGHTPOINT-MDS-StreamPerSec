using System;
using System.Collections.Generic;
using System.Text;

namespace CommandProtocol.Requestable
{
    public interface IRequestable
    {

    }

    public interface IProcessableRequest : IRequestable
    {
        String ConnectionId { get; set; }
        String CorrelationId { get; set; }
    }

    public enum RequestType
    {
        Reference = 0,
        Subscription = 1,
        Search = 2,
        UnSubscribe = 3,
    }

    public class IncomingRequest : IProcessableRequest
    {
        public String ConnectionId { get; set; }
        public String CorrelationId { get; set; }
        public String Version { get; set; }
        public String Requestor { get; set; }
        public String Timestamp { get; set; }
        public String Datasource { get; set; }
        public String UserId { get; set; }
        public RequestBag RequestBag { get; set; }
        public RequestType requestType { get; set; }
    }

    public class RequestBag : IRequestable
    {
        public List<SecurityDefinition> Securities { get; set; } = new List<SecurityDefinition>();
        public List<string> Fields { get; set; } = new List<string>();
        public int Rate { get; set; }
        public string Options { get; set; }
        public string Keyword { get; set; }
    }

    public class SecurityDefinition : IRequestable
    {
        public String SecurityIdentifier { get; set; }
        public String IdentifierType { get; set; }
    }

}
