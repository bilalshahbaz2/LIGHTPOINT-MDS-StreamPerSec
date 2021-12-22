using System;
using System.Collections.Generic;
using System.Linq;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;

namespace CommandProtocol.Extenstions
{
    public static class LoggerRequestableExtension
    {
        public static Object ToLogger(this IncomingRequest incomingRequest)
        {
            List<String> tickers = new List<string>();
            if (incomingRequest.requestType != RequestType.Search) {
                tickers = incomingRequest.RequestBag.Securities.Select(s => s.SecurityIdentifier).ToList();

            }
            return new
            {
                RequestType = incomingRequest.requestType,
                CorrelationId = incomingRequest.CorrelationId,
                Timestamp = incomingRequest.Timestamp,
                Keyword = incomingRequest.RequestBag.Keyword,
                Tickers = String.Join(", ", tickers)

            };
        }

        public static Object FieldsLogger( this OutgoingMessage outgoingMessage)
        {
            if (outgoingMessage == null) {
                return new
                {
                    message = "NULL OBJECT"
                };
            }

            List<ResponseBagItem> items = outgoingMessage.ResponseBag.Items;
            if(  outgoingMessage.ResponseBag is null || outgoingMessage.ResponseBag.Items is null || outgoingMessage.ResponseBag.Items.Count == 0)
            {
                return new
                {
                    message = "No Items found in response bag"
                };
            }
            var fields = items.FirstOrDefault().FieldValues.Select(f => $"[{f.Key}]==[{f.Value.Value}]").ToList();

            foreach (ResponseBagItem item in items) {

            }
            return new
            {
                feilds = String.Join(",", fields)
            };
        }

        public static Object TickersLogger(this OutgoingMessage outgoingMessage)
        {
            if (outgoingMessage == null)
            {
                return new
                {
                    message = "NULL OBJECT"
                };
            }

            List<ResponseBagItem> items = outgoingMessage.ResponseBag.Items;
            List<String> tickers = items.Select(item => item.Security.SecurityIdentifier).ToList();
            return new
            {
                Tickers = String.Join(",", tickers),
            };
        }

        public static Object ToLogger(this Transferable.OutgoingMessage outgoingMessage)
        {
            return new
            {
                RequestType = outgoingMessage.RequestType,
                CorrelatinId = outgoingMessage.CorrelationId,
                ConnectionId = outgoingMessage.ConnectionId
            };
        }
    }
}
