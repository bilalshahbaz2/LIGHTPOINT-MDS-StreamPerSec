using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using CommandProtocol.Extenstions;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using MassTransit;
using Microsoft.AspNetCore.SignalR.Client;
using NLog;
using RequestCreator;
using wsClient.Executors;

namespace wsClient.CollectorClient
{
    public class SignalRCollectorClient
    {
        private readonly String hubUrl;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly HubConnection hubConnection;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly CancellationToken cancellationToken;
        private RequestPipeline requestPipeline;
        private Faker faker;

        private bool flag;
        private Timer time;
        private readonly List<OutgoingMessage> minData;
        private int currentSec;

        private int counter;

        public SignalRCollectorClient(String url, CancellationTokenSource cts)
        {
            this.faker = new Faker();
            this.hubUrl = url;
            this.cancellationTokenSource = cts;
            this.cancellationToken = this.cancellationTokenSource.Token;

            this.flag = true;
            this.minData = new List<OutgoingMessage>();
            this.currentSec = 1;

            this.counter = 0;

            this.hubConnection = new HubConnectionBuilder().WithUrl(hubUrl)
               .WithAutomaticReconnect()
               .Build();

            this.InitAsync().GetAwaiter().GetResult();
        }

        private async Task InitAsync()
        {
            await this.hubConnection.StartAsync(this.cancellationToken);
            this.requestPipeline = new RequestPipeline(this.hubConnection, cancellationToken);
            this.RegisterCalback();

            hubConnection.On<KeepAliveMessage>("OnMessage", (message) =>
            {
                logger.Info("FROM SERVER :: {0}", message);
            });
        }

        private void RegisterCalback()
        {
            _ = Task.Factory.StartNew(async () =>
            {
                var stream = hubConnection.StreamAsync<OutgoingMessage>("FeedHandler", cancellationTokenSource.Token);
                var channel = await hubConnection.StreamAsChannelAsync<OutgoingMessage>("FeedHandler", cancellationToken);
                while (await channel.WaitToReadAsync())
                {
                    OutgoingMessage message;
                    while (channel.TryRead(out message))
                    {
                        //logger.Info("Received Message @{r}, @{tickers}, @{fields}", message.ToLogger(), message.TickersLogger(), message.FieldsLogger());

                        if (message.RequestType == CommandProtocol.Requestable.RequestType.Search)
                        {
                            requestPipeline.Search(message);
                        }
                        else if (message.RequestType == CommandProtocol.Requestable.RequestType.Reference)
                        {
                            requestPipeline.Subsciption(message);
                        }
                        if (flag)
                        {
                            this.flag = false;
                            this.time = new Timer(minCount, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
                        }
                        this.minData.Add(message);

                        //var obj = message.ResponseBag.Items[0];

                        //var askValue = obj.FieldValues.ContainsKey("ASK") ? obj.FieldValues["ASK"].Value : "";
                        //var bidValue = obj.FieldValues.ContainsKey("BID") ? obj.FieldValues["BID"].Value : "";

                        //Console.SetCursorPosition(0, 15);
                        //Console.WriteLine($"{this.currentSec}");
                        //Console.WriteLine($"-- ASK = {askValue}");
                        //Console.WriteLine($"-- BID = {bidValue}");
                        //Console.WriteLine("");
                    }
                }
            });

        }

        public async Task Execute()

        {

            //var referenceRequest = RequestHelper.ReferenceData( new List<String> { "AA-US", "AAPL-US", "MSFT-US" });
            //requestPipeline.Send(referenceRequest);

            //SendSearch(requestPipeline);

            SubscrptionRequest();

            //var text = System.IO.File.ReadAllText(@"C:\src\bshabaz-mds\lightpoint-core\Server\LP.MDS.Collectors\src\Collectors\Factset\Factset.Markers\tickers.txt");


            //await this.doBulkExecutionAsync();
        }


        private void SubscrptionRequest()
        {
            string[] tickers = File.ReadAllLines(Environment.CurrentDirectory + @"\tickers.txt");
            var subscriptionRequest = RequestHelper.Subscription(new List<String>(tickers));

            this.requestPipeline.Send(subscriptionRequest);

            //var subscriptionRequest = RequestHelper.Subscription(new List<String> { "AAPL-US" });
        }


        private void SendSearch(RequestPipeline pipeline)
        {
            //var letterList = Enumerable.Range('A', 'D' - 'A' + 1).
            //          Select(c => (char)c).ToList();

            //string[] prefixes = new string[] { "AA", "AB", "AC", "BA", "BB", "BC" };
            string[] prefixes = new string[] { "AAA" };

            foreach (var _p in prefixes)
            {
                //foreach (var alpha in letterList)
                //{
                //var incomingRequest = RequestHelper.Search($"{_p}{alpha}");
                var incomingRequest = RequestHelper.Search($"{_p}");
                pipeline.Send(incomingRequest);
                //}
            }
        }


        private async Task doBulkExecutionAsync()
        {
            var letterList = Enumerable.Range('A', 'Z' - 'A' + 1).Select(c => (char)c).ToList();

            var pairList = Enumerable.Range('A', 'Z' - 'A' + 1).Select(c => (char)c).ToList();

            List<string> prefixes = new List<String>();
            foreach (var p in pairList)
            {
                string prefix = $"{p}{p}";
                prefixes.Add(prefix);
            }

            var randElements = this.faker.PickRandom(prefixes, 10);

            foreach (var _p in randElements)
            {
                foreach (var alpha in letterList)
                {
                    logger.Info("Sending Search Command @{s}", new { s = $"{_p}{alpha}" });
                    var incomingRequest = RequestHelper.Search($"{_p}{alpha}");
                    requestPipeline.Send(incomingRequest);
                    await Task.Delay(500);
                }
            }
        }

        private void minCount(object state)
        {
            Console.WriteLine($"{this.currentSec}--{this.minData.Count}");
            //var count = this.minData.Count;
            //if (count > 0)
            //{
            //    var obj = this.minData[count - 1].ResponseBag.Items[0];

            //    var askValue = obj.FieldValues.ContainsKey("ASK") ? obj.FieldValues["ASK"].Value : "";
            //    var bidValue = obj.FieldValues.ContainsKey("BID") ? obj.FieldValues["BID"].Value : "";

            //    Console.SetCursorPosition(0, 15);
            //    Console.WriteLine($"{this.currentSec}");
            //    Console.WriteLine($"-- ASK = {askValue}");
            //    Console.WriteLine($"-- BID = {bidValue}");
            //    Console.WriteLine("");
            //}
            this.minData.Clear();
            currentSec++;

        }

    }
}