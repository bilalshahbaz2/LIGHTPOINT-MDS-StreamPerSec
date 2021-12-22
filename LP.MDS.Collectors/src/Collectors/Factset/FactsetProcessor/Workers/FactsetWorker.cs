using CommandProtocol.Extenstions;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using FactSet.Datafeed;
using FactsetProcessor.Mappers;
using gRPCFactset;
using Microsoft.Extensions.Hosting;
using NLog;
using ServiceProtocol.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace FactsetProcessor.Workers
{
    public class FactsetWorker : IHostedService
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        //private readonly IFieldMapper fieldMapper = new FactsetFieldMapper();

        private TransformBlock<Tuple<FactsetRequest, RealtimeEventArgs>, OutgoingMessage> transformResponse;
        private ActionBlock<OutgoingMessage> publishBlock;

        private readonly IClientPublishableService clientPublishableService;
        private readonly IFieldMapper fieldMapper;
        private readonly FactsetSubscriptionDispatcher factsetSubscriptionDispatcher;
        private readonly FactsetConfiguration factsetConfiguration;
        private readonly IFactsetTickerMapper tickerMapper;
        private OnMessageDelegate cb;

        private ConcurrentDictionary<RTSubscription, FactsetRequest> subscriptionCache;
        private BufferBlock<IncomingRequest> incomingbuffer;
        private ActionBlock<IncomingRequest> actionBlock;


        private BufferBlock<IncomingRequest> unSubBuffer;
        private ActionBlock<IncomingRequest> unSubActionBlock;



        public FactsetWorker(FactsetSubscriptionDispatcher _factsetSubscriptionDispatcher,
                            IClientPublishableService clientPublishableService, IFieldMapper fieldMapper,
                            IFactsetTickerMapper tickerMapper, FactsetConfiguration factsetConfiguration)
        {
            this.factsetSubscriptionDispatcher = _factsetSubscriptionDispatcher;
            this.clientPublishableService = clientPublishableService;
            this.factsetConfiguration = factsetConfiguration;
            fieldMapper = fieldMapper;
            this.tickerMapper = tickerMapper;
            this.subscriptionCache = new ConcurrentDictionary<RTSubscription, FactsetRequest>();

            this.transformResponse = new TransformBlock<Tuple<FactsetRequest, RealtimeEventArgs>, OutgoingMessage>((response) =>
            {
                return this.MapToPublishableMsg(response.Item1, response.Item2);
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount });

            this.publishBlock = new ActionBlock<OutgoingMessage>((publishableMsg) =>
            {
                this.clientPublishableService.Publish(publishableMsg);
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount });

            this.transformResponse.LinkTo(this.publishBlock, new DataflowLinkOptions() { PropagateCompletion = true });

            this.actionBlock = new ActionBlock<IncomingRequest>(request => {
                this.Subsciption(request);
            }, new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                //BoundedCapacity = 1
            });

            incomingbuffer = new BufferBlock<IncomingRequest>();

            incomingbuffer.LinkTo(this.actionBlock, new DataflowLinkOptions() { PropagateCompletion = true });



            this.unSubActionBlock = new ActionBlock<IncomingRequest>(request => {
                this.UnSubsciption(request);
            }, new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                //BoundedCapacity = 1
            });

            unSubBuffer = new BufferBlock<IncomingRequest>();

            unSubBuffer.LinkTo(this.unSubActionBlock, new DataflowLinkOptions() { PropagateCompletion = true });


            this.Connect();
        }

        private Task<OutgoingMessage> MapToPublishableMsg(FactsetRequest factsetRequest, RealtimeEventArgs itemEvent)
        {
            var responseBag = fieldMapper.ToSourceCollector(itemEvent.fields, factsetRequest.incomingRequest);
            var result = factsetRequest.incomingRequest.ToOutgoingMessage();
            result.ResponseBag = responseBag;
            return Task.FromResult(result);
        }


        private void Subscibe(CancellationToken cancellationToken)
        {

            Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var request = await factsetSubscriptionDispatcher.recieve();
                    //sslogger.Info("Processing request in worker. @{r},", request.ToLogger());
                    if (request.requestType == CommandProtocol.Requestable.RequestType.UnSubscribe)
                    {
                        //this.UnSubsciption(request);
                        this.unSubBuffer.Post(request);
                    }
                    else
                    {
                        //this.Subsciption(request);
                        this.incomingbuffer.Post(request);
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        private void UnSubsciption(IncomingRequest request)
        {
            List<RTSubscription> unsubscribed = new List<RTSubscription>();
            Dictionary<RTSubscription, FactsetRequest> subscriptionData = new Dictionary<RTSubscription, FactsetRequest>(subscriptionCache);
            foreach (var item in subscriptionData)
            {
                if (item.Value.incomingRequest.CorrelationId == request.CorrelationId || item.Value.incomingRequest.ConnectionId == request.ConnectionId)
                    unsubscribed.Add(item.Key);
            }
            foreach (var item in unsubscribed)
            {

                FDF.Cancel(item);
                logger.Info("Unsubscribed: {0}", item);

            }

        }

        private void Subsciption(IncomingRequest request)
        {
            logger.Info("Processing request in Factset worker. @{r},", request.ToLogger());
            RTRequest req = null;
            foreach (var security in request.RequestBag.Securities)
            {
                var factsetRequest = new FactsetRequest(request, security.SecurityIdentifier, tickerMapper.toFactSet(security.SecurityIdentifier));
                logger.Info("Factset worker subscribe method called.");
                req = new RTRequest("FDS1", factsetRequest.FactsetTicker + ":D");
                RTSubscription sub = FDF.Request(req, delegate (RTSubscription rtSubscription, RTMessage rTMessage, RTRecord rtRecord)
                {
                    var factsetrequest = subscriptionCache.GetValueOrDefault(rtSubscription);
                    if (rTMessage.IsError)
                    {
                        logger.Error("Subscription Error @{error}, Request @{r}", new
                        {
                            Error = rTMessage.Error,
                            ErrorDescription = rTMessage.ErrorDescription,
                        }, factsetrequest.ToLogger());
                    }
                    else
                    {
                        logger.Debug("Callback response from Factset @{r}", factsetrequest.ToLogger());
                        RealtimeEventArgs itemEvent = new RealtimeEventArgs();

                        foreach (RTFidField fld in rTMessage)
                        {
                            string fieldName = RTFieldMap.GetFieldName(fld.Id);
                            itemEvent.fields.Add(fieldName, fld.Value);
                        }
                        transformResponse.Post(Tuple.Create(factsetrequest, itemEvent));
                    }
                });

                bool isSubscriptionCached = false;
                while (!isSubscriptionCached)
                {
                    isSubscriptionCached = subscriptionCache.TryAdd(sub, factsetRequest);
                }
            }
            logger.Info("Done with Processing. @{r},", request.ToLogger());
        }

        private void ProcessHandler(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested && FDF.IsConnected)
                {

                    FDF.Dispatch(1000);
                }
            });
        }

        private void Connect()
        {
            RTFieldMap.Create(factsetConfiguration.RTFieldFile);

            FDF.ConnInfo = factsetConfiguration.SubscriptionConnectionString;
            FDF.OptionsGreeksEnabled = true;
            FDF.Connect();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.Info("Starting hosted service");
            this.ProcessHandler(cancellationToken);

            this.Subscibe(cancellationToken);

            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            FDF.Disconnect();
            return Task.CompletedTask;
        }



    }
}