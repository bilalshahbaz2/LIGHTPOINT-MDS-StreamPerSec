using System;
using System.Collections.Generic;
using CommandProtocol.Extenstions;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using ProcessorProtocol;
using ServiceProtocol.Services;

namespace VisualsetProcessor.Processor
{
    public class SubscriptionDataProcessorImpl : MarketDataProcessor
    {
        private readonly SubscriptionManager subsciptionManager;
        private readonly IClientPublishableService clientPublisher;

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public SubscriptionDataProcessorImpl(SubscriptionManager subsciptionManager,
            IClientPublishableService clientPublisher)
        {
            this.subsciptionManager = subsciptionManager;
            this.clientPublisher = clientPublisher;
        }

        /**
         * 
         * 
         * 
         */
        public void HandleEvent(RealtimeEventArgs arg)
        {
            logger.Info($"Recevied an RealtimeArg Event - {arg.ticker}");
            List<string> subscriptions = subsciptionManager.GetSubscriptions(arg.ticker);
            if(subscriptions != null && subscriptions.Count > 0)
            {
                logger.Info("Subscriptions @{s}", subscriptions);
                foreach(var subCorrelationId in subscriptions)
                {
                    IncomingRequest incomingRequest = subsciptionManager.FindByCorrelationId(subCorrelationId);
                    OutgoingMessage publishableMessage = MapToOutgoingResponse(arg, incomingRequest);

                    clientPublisher.Publish(publishableMessage);
                }
            }
        }

        private OutgoingMessage MapToOutgoingResponse(RealtimeEventArgs arg, IncomingRequest request)
        {
            OutgoingMessage outgoingMessage = request.ToOutgoingMessage();
            outgoingMessage.ResponseBag = new ResponseBag();

            CommandProtocol.Transferable.SecurityDefinition securityDefinition = new CommandProtocol.Transferable.SecurityDefinition();
            securityDefinition.SecurityIdentifier = arg.ticker;
            securityDefinition.IdentifierType = "TICKER";

            ResponseBagItem item = new ResponseBagItem();
            item.Security = securityDefinition;
            item.SequenceNo = "0x000000";
            
            foreach (var field in arg.fields)
            {
                FieldDescriptor fd = new FieldDescriptor();
                fd.Key = field.Key;
                fd.Value = field.Value;
                fd.HasError = false;
                fd.OriginatingSource = "VisualSet";
                fd.Timestamp = DateTime.UtcNow.ToString();
                fd.CollectorCode = $"{field}-VisualSet";
                item.FieldValues.Add(field.Key, fd);
                
            }
            outgoingMessage.ResponseBag.Items.Add(item);

            return outgoingMessage;
        }
       
        public void PostAsync(IncomingRequest incomingRequest)
        {
             incomingRequest.ToAckMessage();
        }

        public OutgoingMessage Post(IncomingRequest incomginRequest)
        {
            return incomginRequest.ToOutgoingMessage();
        }
    }
}
