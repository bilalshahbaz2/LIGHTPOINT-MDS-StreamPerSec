using System;
using System.Collections.Generic;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;

namespace CommandProtocol.Extenstions
{
    public static class ProcessableRequestExtension
    {
        public static OutgoingMessage ToOutgoingMessage(this IncomingRequest incomingRequest, RealtimeEventArgs arg)
        {
            OutgoingMessage outgoingMessage = incomingRequest.ToOutgoingMessage();

            outgoingMessage.ResponseBag = new ResponseBag();

            ResponseBagItem responseBagItem = new ResponseBagItem();
            outgoingMessage.ResponseBag.Items = new List<ResponseBagItem>();

            Transferable.SecurityDefinition securityDefinition = new Transferable.SecurityDefinition();

            securityDefinition.SecurityIdentifier = arg.ticker;
            securityDefinition.LastUpdate = DateTime.Now.ToString();
            securityDefinition.Message = "Response from Factset";
            securityDefinition.IdentifierType = "Ticker";

            responseBagItem.SequenceNo = "0x000000";

            Dictionary<string, FieldDescriptor> fields = new Dictionary<string, FieldDescriptor>();

            foreach (var field in arg.fields)
            {
                fields.Add(field.Key, new FieldDescriptor
                {
                    Value = field.Value,
                    CollectorCode = string.Empty,
                    HasError = false,
                    Message = "Response from Factset",
                    OriginatingSource = "Factset",
                    Timestamp = DateTime.Now.ToString(),
                    Key = string.Empty
                });
            }

            Dictionary<string, string> fielderrors = new Dictionary<string, string>();
            responseBagItem.Security = securityDefinition;
            responseBagItem.FieldValues = fields;
            responseBagItem.FieldErrors = fielderrors;

            outgoingMessage.ResponseBag.Items.Add(responseBagItem);

            return outgoingMessage;
        }

        public static OutgoingMessage ToOutgoingMessage(this IncomingRequest incomingRequest)
        {
            OutgoingMessage outgoingMessage = new OutgoingMessage();
            outgoingMessage.Requestor = incomingRequest.Requestor;
            outgoingMessage.Timestamp = DateTime.UtcNow.ToString();
            outgoingMessage.UserId = incomingRequest.UserId;
            outgoingMessage.Version = incomingRequest.Version;
            outgoingMessage.Datasource = incomingRequest.Datasource;
            outgoingMessage.CorrelationId = incomingRequest.CorrelationId;
            outgoingMessage.ConnectionId = incomingRequest.ConnectionId;
            outgoingMessage.RequestType = incomingRequest.requestType;

            return outgoingMessage;
        }

        public static AckMessage ToAckMessage(this IncomingRequest incomingRequest)
        {
            AckMessage ackMessage = new AckMessage();

            ackMessage.ConnectionId = incomingRequest.ConnectionId;
            ackMessage.CorrelationId = incomingRequest.CorrelationId;
            ackMessage.Datasource = incomingRequest.Datasource;
            ackMessage.Message = string.Empty;
            ackMessage.Requestor = incomingRequest.Requestor;
            ackMessage.Timestamp = DateTime.Now.ToString();
            ackMessage.UserId = incomingRequest.UserId;
            ackMessage.Version = incomingRequest.Version;

            return ackMessage;
        }

        public static OutgoingMessage ToOutgoingMessage(this IncomingRequest incomingRequest, SearchEventArgs searchEventArgs)
        {
            OutgoingMessage outgoingMessage = incomingRequest.ToOutgoingMessage();

            outgoingMessage.ResponseBag = new ResponseBag();
            outgoingMessage.ResponseBag.Items = new List<ResponseBagItem>();

            foreach (var item in searchEventArgs.TypeaHead.Results)
            {
                var responsebagitem = new ResponseBagItem();
                responsebagitem.Security = new Transferable.SecurityDefinition();
                responsebagitem.Security.SecurityIdentifier = item.Symbol;
                responsebagitem.Security.LastUpdate = DateTime.Now.ToString();
                responsebagitem.Security.Message = item.Name;
                responsebagitem.Security.IdentifierType = item.Index;

                /**
                 * Logic needs to be implemented so that we can map Fields with respect to Bloomberg Menmonics
                 */
                //foreach (var field in incomingRequest.RequestBag.Fields) {
                //    responsebagitem.FieldValues.Add(field, new FieldDescriptor { 
                //        Key = item.Fds_id, Value = item.Ticker 
                //    });
                //}

                //responsebagitem.FieldValues.Add(item.Entity_id, new FieldDescriptor { Key = item.Fds_id, Value = item.Ticker });
                responsebagitem.FieldValues.Add("Key", new FieldDescriptor { Key = "Key", Value = item.Fds_id });
                responsebagitem.FieldValues.Add("Symbol", new FieldDescriptor { Key = "Symbol", Value = item.Symbol });
                responsebagitem.FieldValues.Add("EntityId", new FieldDescriptor { Key = "EntityId", Value = item.Entity_id });
                responsebagitem.FieldValues.Add("Ticker", new FieldDescriptor { Key = "Ticker", Value = item.Ticker });
                responsebagitem.FieldValues.Add("FdsId", new FieldDescriptor { Key = "FdsId", Value = item.Fds_id });


                responsebagitem.FieldValues.Add("SECURITY_TYP", new FieldDescriptor { Key = "SECURITY_TYP", Value = string.Empty });
                responsebagitem.FieldValues.Add("CRNCY", new FieldDescriptor { Key = "CRNCY", Value = string.Empty });
                responsebagitem.FieldValues.Add("EQY_PRIM_EXCH_SHRT", new FieldDescriptor { Key = "EQY_PRIM_EXCH_SHRT", Value = string.Empty });
                responsebagitem.FieldValues.Add("SHORT_NAME", new FieldDescriptor { Key = "SHORT_NAME", Value = item.Name });
                responsebagitem.FieldValues.Add("MARKET_SECTOR_DES", new FieldDescriptor { Key = "MARKET_SECTOR_DES", Value = string.Empty });
                responsebagitem.FieldValues.Add("SECURITY_NAME", new FieldDescriptor { Key = "SECURITY_NAME", Value = item.Name });
                responsebagitem.FieldValues.Add("SECURITY_DES", new FieldDescriptor { Key = "SECURITY_DES", Value = string.Empty });

                outgoingMessage.ResponseBag.Items.Add(responsebagitem);
            }

            return outgoingMessage;
        }

        public static OutgoingMessage ToOutgoingMessage(this IncomingRequest request, Dictionary<string, Dictionary<string, string>> incomingResult)
        {
            OutgoingMessage outgoingMessage = request.ToOutgoingMessage();

            outgoingMessage.ResponseBag = new ResponseBag();
            outgoingMessage.ResponseBag.Items = new List<ResponseBagItem>();

            foreach (var item in incomingResult)
            {
                var responsebagitem = new ResponseBagItem();
                responsebagitem.Security = new CommandProtocol.Transferable.SecurityDefinition();

                responsebagitem.Security.IdentifierType = "ticker";
                responsebagitem.Security.LastUpdate = DateTime.Now.ToString();
                responsebagitem.Security.Message = "Ref data.";
                responsebagitem.Security.SecurityIdentifier = item.Key;

                responsebagitem.FieldErrors = new Dictionary<string, string>();

                var pairs = item.Value;

                foreach (var pair in pairs)
                {
                    responsebagitem.FieldValues.Add(pair.Key, new FieldDescriptor
                    {
                        CollectorCode = string.Empty,
                        Message = string.Empty,
                        Value = pair.Value,
                        HasError = false,
                        Key = string.Empty,
                        OriginatingSource = "factset",
                        Timestamp = DateTime.Now.ToString()
                    });
                }


                outgoingMessage.ResponseBag.Items.Add(responsebagitem);
            }

            return outgoingMessage;
        }
    }
}
