﻿using CommandProtocol.Extenstions;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using Microsoft.AspNetCore.SignalR.Client;
using NLog;
using RequestCreator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace wsClient.Executors
{
    public class RequestPipeline
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly HubConnection hubConnection;
        private readonly CancellationToken cancellationToken;

        private BufferBlock<OutgoingMessage> searchBlock;
        private TransformBlock<OutgoingMessage, IncomingRequest> searchTransformBlock;

        private TransformBlock<OutgoingMessage, IncomingRequest> subscriptionTransformBlock;


        private BufferBlock<IncomingRequest> IncomingRequestBuffer;

        private ActionBlock<IncomingRequest> RequestActionBlock;
        private ActionBlock<IncomingRequest> subscriptionActionBlock;

        private static DataflowLinkOptions linkOptions()
        {

            return new DataflowLinkOptions() { PropagateCompletion = true };
        }

        private static ExecutionDataflowBlockOptions executionOptions()
        {
            return new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
        }

        private static DataflowBlockOptions DataflowBlockOptions()
        {
            return new DataflowBlockOptions() { };
        }

        public RequestPipeline(HubConnection hubConnection, CancellationToken cancellationToken)
        {
            this.hubConnection = hubConnection;
            this.cancellationToken = cancellationToken;

            this.searchBlock = new BufferBlock<OutgoingMessage>();
            this.searchTransformBlock = new TransformBlock<OutgoingMessage, IncomingRequest>(message => this.ProcessSearchResult(message), executionOptions());

            this.IncomingRequestBuffer = new BufferBlock<IncomingRequest>(DataflowBlockOptions());
            this.RequestActionBlock = new ActionBlock<IncomingRequest>(request => this.ProcessRequest(request), executionOptions());

            subscriptionTransformBlock = new TransformBlock<OutgoingMessage, IncomingRequest>( message => this.SubscriptionRequest(message), executionOptions());
            subscriptionActionBlock = new ActionBlock<IncomingRequest>( request => this.ProcessRequest(request), executionOptions());

            subscriptionTransformBlock.LinkTo(subscriptionActionBlock, linkOptions());

            this.searchBlock.LinkTo(searchTransformBlock, linkOptions());
            this.searchTransformBlock.LinkTo(RequestActionBlock, linkOptions());

            this.IncomingRequestBuffer.LinkTo(RequestActionBlock, linkOptions(), item => ( item.requestType != RequestType.Subscription ) );

            this.IncomingRequestBuffer.LinkTo(subscriptionActionBlock, linkOptions(), item => item.requestType == RequestType.Subscription);
        }

        private IncomingRequest SubscriptionRequest(OutgoingMessage message)
        {
            List<String> tickers = new List<string>();
            if (message.ResponseBag != null && message.ResponseBag.Items != null)
            {
                tickers = message.ResponseBag.Items.Select(item => item.Security.SecurityIdentifier).ToList();
                return RequestHelper.Subscription(tickers.Distinct().ToList());
            }

            return RequestHelper.Subscription(tickers);
        }

        private IncomingRequest ProcessSearchResult(OutgoingMessage message)
        {
            List<String> tickers = new List<string>();
            if (message.ResponseBag != null && message.ResponseBag.Items != null)
            {
                foreach (ResponseBagItem item in message.ResponseBag.Items)
                {
                    var keyFieldDescriptor = item.FieldValues.GetValueOrDefault("Symbol");

                    logger.Info(" Key Field @{f}", new
                    {
                        Key = keyFieldDescriptor.Key,
                        Value = keyFieldDescriptor.Value
                    });
                    if ( !String.IsNullOrEmpty(keyFieldDescriptor.Value))
                    {
                        tickers.Add(keyFieldDescriptor.Value);
                    }
                }

                return RequestHelper.ReferenceData(tickers.Distinct().ToList());
            }

            return RequestHelper.ReferenceData(tickers);
        }

        private void ProcessRequest(IncomingRequest r)
        {
            if(r.requestType == RequestType.Subscription)
            {
                logger.Info("Doing Subscription @{r}", r.ToLogger());
                
            }
            else
            {
                logger.Info("Do Reference Data Call for @{r}", r.ToLogger());
            }
            hubConnection.SendAsync("Send", r, this.cancellationToken);

        }

        public void Send(IncomingRequest request)
        {
            logger.Info("Sending Request @{r}", request.ToLogger() );
            this.IncomingRequestBuffer.Post(request);
        }

        public void Search(OutgoingMessage message)
        {
            this.searchBlock.Post(message);
        }

        public void Subsciption(OutgoingMessage message)
        {
            this.subscriptionTransformBlock.Post(message);
        }
    }
}
