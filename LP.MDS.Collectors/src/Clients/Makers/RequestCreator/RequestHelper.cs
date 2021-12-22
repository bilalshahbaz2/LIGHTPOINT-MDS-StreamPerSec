using CommandProtocol.Requestable;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestCreator
{
    public class RequestHelper
    {

        public static IncomingRequest Search(string text)
        {
            var incomingrequest = GetComposedRequest(RequestType.Search);
            incomingrequest.RequestBag.Keyword = text;
            incomingrequest.RequestBag.Fields = new String[] {
                    "SHORT_NAME",
                    "SECURITY_DES",
                    "SECURITY_NAME",
                    "SECURITY_TYP",
                    "CRNCY",
                    "EQY_PRIM_EXCH_SHRT",
                    "MARKET_SECTOR_DES"
            }.ToList();
            return incomingrequest;
        }

        public static IncomingRequest ReferenceData(List<string> tickers)
        {
            var request = GetComposedRequest(RequestType.Reference);
            request.RequestBag.Securities = CreateSecurity(tickers);
            return request;
        }


        public static IncomingRequest Subscription(List<string> tickers)
        {
            var request = GetComposedRequest(RequestType.Subscription);
            request.RequestBag.Securities = CreateSecurity(tickers);
            return request;
        }


        private static List<SecurityDefinition> CreateSecurity(List<String> tickers)
        {
            List<SecurityDefinition> securities = new List<SecurityDefinition>();
            foreach( var t in tickers)
            {
                securities.Add(CreateSecurity(t));
            }
            return securities;
        }

        private static SecurityDefinition CreateSecurity(String ticker)
        {
            SecurityDefinition security = new SecurityDefinition() {
                IdentifierType = "TICKER",
                SecurityIdentifier = ticker
            };
            return security;
        }

        private static IncomingRequest GetComposedRequest(RequestType requestType)
        {
            List<String> menmmomincs = new List<string>()
            {
                //"PX_LAST",
                //"SEUCRITY_TYPE",
                "BID",
                "ASK"
                //"BID_SIZE",
                //"ASK_SIZE"
            };

            IncomingRequest r = new IncomingRequest();
            r.CorrelationId = NewId.NextGuid().ToString();
            r.Datasource = "Factset";
            r.Requestor = "TestApp";
            r.UserId = "hali-lightpointft";
            r.Timestamp = DateTime.Now.ToString();
            r.Version = "1.01";
            r.requestType = requestType;
            r.RequestBag = new RequestBag();
           

            r.RequestBag.Fields = new List<string>();
            r.RequestBag.Fields = menmmomincs;

            return r;
        }

    }
}
