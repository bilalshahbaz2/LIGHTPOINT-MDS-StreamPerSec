using CommandProtocol.Extenstions;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using ProcessorProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Services
{
    public class UnSubscribeService : IUnSubscribeService
    {
        private readonly SubscriptionManager subscriptionManager;
        private readonly MarketDataProcessor marketDataProcessor;

        public UnSubscribeService(SubscriptionManager _subscriptionManager, MarketDataProcessor _marketDataProcessor)
        {
            subscriptionManager = _subscriptionManager;
            marketDataProcessor = _marketDataProcessor;
        }

        public void PostAsync(IncomingRequest incomingRequest)
        {

            /*
             * bussiness logic
             * 
             * 
             */
            this.subscriptionManager.RemoveByConnectionId(incomingRequest.ConnectionId);
            marketDataProcessor.PostAsync(incomingRequest);
        }
    }
}
