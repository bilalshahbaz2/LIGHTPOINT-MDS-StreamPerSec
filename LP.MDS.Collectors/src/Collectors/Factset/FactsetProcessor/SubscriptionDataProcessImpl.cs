using CommandProtocol.Extenstions;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using FactsetProcessor.proxy;
using FactsetProcessor.V2.FactsetProxy;
using FactsetProcessor.Workers;
using NLog;
using ProcessorProtocol;
using ServiceProtocol.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace FactsetProcessor
{
    public class SubscriptionDataProcessImpl : MarketDataProcessor
    {
        //private FactsetProxy _proxy = new FactsetProxy();
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IClientPublishableService publishableService;
        private readonly IFactsetProxy factsetProxy;
        private readonly SubscriptionManager subscriptionManager;
        private readonly FactsetSubscriptionDispatcher factsetSubscriptionDispatcher;

        public SubscriptionDataProcessImpl(IClientPublishableService _publishableService, IFactsetProxy _factsetProxy,
            SubscriptionManager _subscriptionManager, FactsetSubscriptionDispatcher _factsetSubscriptionDispatcher)
        {
            publishableService = _publishableService;
            factsetProxy = _factsetProxy;
            subscriptionManager = _subscriptionManager;
            factsetSubscriptionDispatcher = _factsetSubscriptionDispatcher;
        }

        private Task<OutgoingMessage> processitem(IncomingRequest request)
        {
            throw new NotImplementedException();
        }

        public void HandleEvent(RealtimeEventArgs arg)
        {
            var incomingresuqset = subscriptionManager.FindByCorrelationId(arg.correlationId);

            var outgoingmessage = incomingresuqset.ToOutgoingMessage();

            publishableService.Publish(outgoingmessage);
        }

        public void PostAsync(IncomingRequest incomingRequest)
        {
            logger.Info($"Request in process level. - Correlation ID = {incomingRequest.CorrelationId}");
            factsetSubscriptionDispatcher.Post(incomingRequest);

            //return incomingRequest.ToAckMessage();
        }

        public OutgoingMessage Post(IncomingRequest incomginRequest)
        {
            //throw new NotImplementedException();
            logger.Info("Request in process level.");
            factsetSubscriptionDispatcher.Post(incomginRequest);
            return null;
        }
    }
    
}
