using CommandProtocol.Requestable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactsetProcessor
{
    public class FactsetRequest
    {
        public IncomingRequest incomingRequest { get; private set; }
        public string FactsetTicker { get; private set; }
        public string BbgTicker { get; private set; }

        public FactsetRequest( IncomingRequest request, String BbgTicker, String factsetTicker)
        {
            this.incomingRequest = request;
            this.BbgTicker = BbgTicker;
            this.FactsetTicker = factsetTicker;
        }

        public Object ToLogger() {
            return new { 
                FacetTicker = this.FactsetTicker,
                BbgTicker = this.BbgTicker,
                CorrelationId = this.incomingRequest.CorrelationId,
                ConnectionId = this.incomingRequest.ConnectionId
            };
        }
    }
}
