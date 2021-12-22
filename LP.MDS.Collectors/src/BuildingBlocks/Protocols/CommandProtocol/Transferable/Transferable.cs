using CommandProtocol.Requestable;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandProtocol.Transferable
{
    public interface ITransferable 
    {
    }

    public interface IPublishable
    {
        String ConnectionId { get; set; }
    }

    public class KeepAliveMessage : ITransferable, IPublishable
    {
        public string ConnectionId { get; set; }
        public String Message { get; set; }
        public String Timestamp { get; set; } = DateTime.UtcNow.ToLongDateString();
    }

    public class OutgoingMessage : ITransferable, IPublishable
    {
        public string Version { get; set; }
        public string Requestor { get; set; }
        public string Timestamp { get; set; }
        public string Datasource { get; set; }
        public string CorrelationId { get; set; }
        public string UserId { get; set; }
        public RequestType RequestType { get; set; }
        public ResponseBag ResponseBag { get; set; } = new ResponseBag();
        public String ConnectionId { get; set; }
    }

    public class ResponseBag : ITransferable
    {
        public List<ResponseBagItem> Items { get; set; } = new List<ResponseBagItem>();
    }

    [Serializable]
    public class ResponseBagItem : ITransferable { 
        
        public SecurityDefinition Security { get; set; }
        public String SequenceNo { get; set; } = "0";
        public Dictionary<string, FieldDescriptor> FieldValues { get; set; } = new Dictionary<string, FieldDescriptor>();
        public Dictionary<String, String> FieldErrors { get; set; } = new Dictionary<string, string>();

    }

    [Serializable]
    public class FieldDescriptor : ITransferable
    {
        public String Key { get; set; } = "";
        public String Value { get; set; } = "";
        public bool HasError { get; set; } = false;
        public String Timestamp { get; set; } = "";
        public String OriginatingSource { get; set; } = "";
        public String Message { get; set; } = "";
        public String CollectorCode { get; set; } = "";
    }

    [Serializable]
    public class SecurityDefinition :  ITransferable
    {
        public string SecurityIdentifier { get; set; }
        public string IdentifierType { get; set; }
        public string Message { get; set; } = "";
        public string LastUpdate { get; set; } = "";
    }

    public class AckMessage : ITransferable, IPublishable
    {
        public string Version { get; set; }
        public string Requestor { get; set; }
        public string Timestamp { get; set; }
        public string Datasource { get; set; }
        public string CorrelationId { get; set; }
        public string UserId { get; set; }
        public string ConnectionId { get; set; }
        public string Message { get; set; }
        
    }

}
