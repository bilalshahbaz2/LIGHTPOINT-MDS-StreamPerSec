using CommandProtocol.Extenstions;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using FluentValidation;
using NLog;
using ProcessorProtocol;
using ServiceProtocol.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceProtocol.Services
{
    public class MarketDataService : IMarketDataService
    {
        private static readonly ILogger logger = LogManager.GetLogger("marketdataservice");
        private readonly SubscriptionManager subscirptionManager;
        private readonly MarketDataProcessor marketDataProcessor;
        private readonly IncomingRequestValidator validator;

        public MarketDataService(SubscriptionManager subscirptionManager, 
            MarketDataProcessor _marketDataProcessor,
            IncomingRequestValidator validator
           )
        {
            this.subscirptionManager = subscirptionManager;
            this.marketDataProcessor = _marketDataProcessor;
            this.validator = validator;
        }

        public bool hasSubscription(IncomingRequest request)
        {
            throw new NotImplementedException();
        }

        
        public bool Unsubscribe(IncomingRequest request)
        {
            this.marketDataProcessor.PostAsync(request);
            return true;
        }

        public OutgoingMessage Post(IncomingRequest request)
        {
            logger.Info($"MarketDataService - POST - CorrelationID = {request.CorrelationId}");
            this.validator.ValidateAndThrow(request);
            this.marketDataProcessor.PostAsync(request);

            this.subscirptionManager.Add(request);

            var outgoingMessage = new OutgoingMessage();
            return outgoingMessage;
        }

        public void PostAsync(IncomingRequest incomingRequest)
        {
            this.validator.ValidateAndThrow(incomingRequest);
            this.Post(incomingRequest);
        }
    }
}
