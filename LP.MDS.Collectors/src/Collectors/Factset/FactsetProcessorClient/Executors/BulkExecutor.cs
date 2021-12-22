using CommandProtocol.Extenstions;
using FactsetProcessor.proxy;
using FactsetProcessor.V2.FactsetProxy;
using NLog;
using RequestCreator;
using ServiceProtocol.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FactsetProcessorClient.Executors
{
    public class BulkExecutor
    {
        private static NLog.ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly RequestPipeline requestFlow;
        private readonly IClientPublishableService publisherService;
        private readonly string ConnectionId;
        private readonly IFactsetProxy factsetProxy;

        public BulkExecutor(RequestPipeline requestFlow,
            IFactsetProxy factsetProxy,
            IClientPublishableService publisherService)
        {
            this.requestFlow = requestFlow;
            this.publisherService = publisherService;
            this.factsetProxy = factsetProxy;
            ConnectionId = "Bulk-Process-Id";

        }

        public Task Execute(CancellationToken cancellationToken)
        {
            publisherService.Register(ConnectionId);
            var task = this.FeedHandler(cancellationToken);

            //this.SubscribeData();

            //this.doProxyRequest();
            //this.SendSearch();

            //this.ReferenceData();


            this.SubscribeData(new List<string>() { "MSFT-US", "AAPL-US","FB-US" });


            return task;
        }

        private void doProxyRequest()
        {
            RequestCreator.RequestHelper.ReferenceData( new string[] { "", ""}.ToList() );

        }

        private void ReferenceData(List<string> symbolList) {
            //var referenceData = new string[] { "ARL-DE", "AA-US" };
            var referenceTypeRequest = RequestHelper.ReferenceData(symbolList);
            referenceTypeRequest.ConnectionId = ConnectionId;
            this.requestFlow.Send(referenceTypeRequest);
        }

        
        private void SubscribeData(List<string> symbolList)
        {
            var subscribeTypeRequest = RequestHelper.Subscription(symbolList);
            subscribeTypeRequest.ConnectionId = ConnectionId;
            AppStartCache.CacheData(subscribeTypeRequest.CorrelationId,subscribeTypeRequest);
            //List<string> keyList = new List<string>(AppStartCache.CachedResults.Keys);
            //foreach (var item in keyList)
            //{
            //    Task.Delay(5000).ContinueWith(t => Unsubscribe(item));
            //}
            this.requestFlow.Send(subscribeTypeRequest);
        }

        public void Unsubscribe(string key)
        {
            var value = AppStartCache.GetCachedData(key);
            value.Item1.requestType = CommandProtocol.Requestable.RequestType.UnSubscribe;
            logger.Info("Processing request in worker. @{r},", value);
            this.requestFlow.Send(value.Item1);
        }
        private void SendSearch() {

            //string[] prefixes = new string[] { "AAA" };

            //foreach (var alpha in prefixes)
            //{
            //    var incomingRequest = RequestHelper.Search($"{alpha}");
            //    incomingRequest.ConnectionId = ConnectionId;
            //    this.requestFlow.Send(incomingRequest);
            //}


            var letterList = Enumerable.Range('A', 'D' - 'A' + 1).
                         Select(c => (char)c).ToList();

            string[] prefixes = new string[] { "AA", "AB", "AC", "BA", "BB", "BC" };

            foreach (var _p in prefixes)
            {
                foreach (var alpha in letterList)
                {
                    var incomingRequest = RequestHelper.Search($"{_p}{alpha}");
                    incomingRequest.ConnectionId = ConnectionId;
                    this.requestFlow.Send(incomingRequest);
                }
            }
        }

        List<string> symbolList = new List<string>();
        private Task FeedHandler( CancellationToken cancellationToken) {
            return Task.Run(async () =>
            {
                logger.Info("FeedHandler has been Registered  ");

                while (!cancellationToken.IsCancellationRequested)
                {
                    CommandProtocol.Transferable.OutgoingMessage message = await this.publisherService.ReceiveAsync(ConnectionId);

                    logger.Info("Received Message @{r}, @{tickers}, @{fields}", message.ToLogger(), message.TickersLogger(), message.FieldsLogger());

                    if (message.RequestType == CommandProtocol.Requestable.RequestType.Search)
                    {
                        
                        foreach (var item in message.ResponseBag.Items)
                        {
                           symbolList.Add(item.FieldValues.FirstOrDefault(x=>x.Key=="Symbol").Value.Value);  
                        };
                        this.ReferenceData(symbolList);

                        //this.SubscribeData(symbolList);

                    }
                    else if (message.RequestType == CommandProtocol.Requestable.RequestType.Reference)
                    {
                        this.SubscribeData(symbolList);
                    }
                }
            });
        }
    }
}
