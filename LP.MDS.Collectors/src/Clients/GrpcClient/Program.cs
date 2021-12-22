using Grpc.Core;
using Grpc.Net.Client;
using GrpcClient.Request;
using GrpcService.MDS;
using MassTransit;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GrpcClient
{
    class Program
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();



        static async Task Main(string[] args)
        {
            ConcurrentDictionary<string, string> correalaitonlist = new ConcurrentDictionary<string, string>(); 

            Console.WriteLine("Applicaiton started. Press Enter.");
            Console.ReadLine();
            Console.WriteLine("Request sent.");

            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

			ReferenceRequest referenceRequest = new ReferenceRequest();
            SearchRequest searchRequest = new SearchRequest();
            SubscriptionRequest subscriptionRequest = new SubscriptionRequest();

            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress("https://localhost:5001");

            var client = new MdsGrpcService.MdsGrpcServiceClient(channel);

            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

			//My Code
            //var referenceRequest = getRequest();
            //client2.GetReferenceData(referenceRequest);
            //
            //
            //
            //
            //
            //

            //BufferBlock<IncomingRequest> bufferBlock = new BufferBlock<IncomingRequest>(new DataflowBlockOptions() { BoundedCapacity = 1 });



            //ActionBlock<IncomingRequest> refblock = new ActionBlock<IncomingRequest>((request) =>
            //{
            //    logger.Info("Request before sent.  @{c}", new { CoorelationId = request.CorrelationId });

            //    correalaitonlist.TryAdd(request.CorrelationId, request.CorrelationId);

            //    //client2.GetReferenceDataSubsciption(request);
            //    client.GetSearchData(request);

            //    logger.Info("Request after sent.  @{c}", new { CoorelationId = request.CorrelationId});


            //}, new ExecutionDataflowBlockOptions() { BoundedCapacity = 1, MaxDegreeOfParallelism = 6 });

            //bufferBlock.LinkTo(refblock, new DataflowLinkOptions() { PropagateCompletion = true });



            //var request = getRequest();


            //ShowResponseMessageOnConsole(response);

            Thread consumerThread = new Thread(async () =>
            {
                var obj = new IncomingRequest();

                using var subsciptionHandler = client.SubscriptionFeedHandler(new Google.Protobuf.WellKnownTypes.Empty());

                logger.Info("Registering CallBack Handler.");

                while (await subsciptionHandler.ResponseStream.MoveNext())
                {
                    OutgoingMessage streamUpdate = subsciptionHandler.ResponseStream.Current as OutgoingMessage;

                    string vab;
                    correalaitonlist.TryRemove(streamUpdate.CorrelationId, out vab);

                    //var tickers = streamUpdate.ResponseBag.Items.Select(item => item.Security.SecurityIdentifier).ToList();
                    foreach (var item in streamUpdate.ResponseBag.Items)
                    {
                        logger.Info($"SecurityIdentifier : {item.Security.SecurityIdentifier}");
                        logger.Info($"IdentifierType : {item.Security.IdentifierType}");
                        logger.Info($"Message : {item.Security.Message}");

                        //foreach (var fields in item.FieldValues)
                        //{
                        //    logger.Info($"Field name : {fields.Key} | Field Value : {fields.Value.Value}");
                        //}
                    }
                }
            });
            consumerThread.Start();

			Console.WriteLine("Request ...");
            var text = Console.ReadLine();
            //if(text.Equals("r"))
            //{
            //Thread thread = new Thread(() =>
            //{
            //    while (true)
            //    {
            //        //logger.Info("Request sent for reference data :");
            referenceRequest.GetRefData(client);
            //        Thread.Sleep(30000);
            //    }

            //});
            //thread.Start();
            //}

            //else if(text.Equals("s"))
            //{
            //logger.Info("subscription request sent.");
            //subscriptionRequest.SubscribeRequest(client);
            //}

            //else if(text.Equals("a"))
            //{

            //Thread thread2 = new Thread(() =>
            //{
            //    while (true)
            //    {
            //        //Console.WriteLine("Enter search text");
            //        //var searchText = Console.ReadLine();
            //        searchRequest.GetSearchData(client, "aap");
            //        Thread.Sleep(40000);
            //    }

            //});
            //thread2.Start();

            //}


            //SearchRequest searchRequest = new SearchRequest();
            //while (true)
            //{
            //    Console.WriteLine("Enter text");
            //    var text = Console.ReadLine();
            //    searchRequest.GetSearchData(client, text);
            //}

            //SubscriptionRequest subscriptionRequest = new SubscriptionRequest();
            //subscriptionRequest.SubscribeRequest(client);



            //for (int i = 0; i < 5; i++)
            //{
            //    Console.WriteLine("Enter text to search : ");
            //    string text = Console.ReadLine();
            //    bufferBlock.Post(getSearchData(text));
            //}

            //bufferBlock.Complete();
            //bufferBlock.Completion.Wait();    

            //logger.Info("Request sent completed");            

            //Console.WriteLine("Subscription request num ? ");
            //string vab = Console.ReadLine();
            //if (vab == "a")
            //{



            //var result = subscriptionRequest(new string[] { "MSFT:D", "DASH:D", "AMD:D" });
            //client.Subscribe(result);
            //}
            //else
            //{
            //    var result = subscriptionRequest(new string[] { "AAPL:D", "MMM:D", "AMZN:D" });
            //    client2.Subscribe(result);
            //}


            /**
             * Subscriptions...
             * 
             */


            //logger.Info("Application stopped.");

            Console.ReadLine();

            Console.ReadLine();

        }


        private static void ShowResponseMessageOnConsole(OutgoingMessage outgoingMessage)
        {
            logger.Info("\n\n\n*********************************FactSet response*********************************");

            logger.Info("CorrelationId  :  " + outgoingMessage.CorrelationId);
            logger.Info("Datasource     :  " + outgoingMessage.Datasource);
            logger.Info("Requestor      :  " + outgoingMessage.Requestor);
            logger.Info("UserId         :  " + outgoingMessage.UserId);
            logger.Info("Version        :  " + outgoingMessage.Version);
            logger.Info("Timestamp      :  " + outgoingMessage.Timestamp);

            foreach (var item in outgoingMessage.ResponseBag.Items)
            {
                logger.Info("\n\n");
                logger.Info("Security IdentifierType       :  " + item.Security.IdentifierType);
                logger.Info("Security Message              :  " + item.Security.Message);
                logger.Info("Security SecurityIdentifier   :  " + item.Security.SecurityIdentifier);

                foreach (var field in item.FieldValues)
                {
                    logger.Info("Mnemonic : " + field.Key
                                    + "\nValue : " + field.Value.Value
                                    + "\nTimestamp : " + field.Value.Timestamp
                                    + "\nOriginatingSource : " + field.Value.OriginatingSource
                                    + "\nHasError : " + field.Value.HasError);
                }
            }

        }

        private static IncomingRequest subscriptionRequest(String ticker)
        {
            return subscriptionRequest(new string[] { ticker });
        }

        private static IncomingRequest subscriptionRequest(String[] ticker)
        {


            List<String> menmmomincs = new List<string>()
            {
                "PX_LAST",
                "SEUCRITY_TYPE",
                "BID",
                "ASK"
            };

            IncomingRequest r = new IncomingRequest();
            r.CorrelationId = NewId.NextGuid().ToString();
            r.Datasource = "Factset";
            r.Requestor = "TestApp";
            r.UserId = "hali-lightpointft";
            r.Timestamp = DateTime.Now.ToString();
            r.Version = "1.01";

            List<SecurityDefinitionRequest> securities = new List<SecurityDefinitionRequest>();
            foreach (var t in ticker)
            {
                securities.Add(new SecurityDefinitionRequest()
                {
                    IdentifierType = "TICKER",
                    SecurityIdentifier = t
                });
            }

            r.RequestBag = new RequestBag();
            r.RequestBag.Securities.Add(securities);

            r.RequestBag.Fields.Add(menmmomincs.ToList());

            return r;
        }


        private static IncomingRequest getSearchData(string text)
        {
            var incomingrequest = getRequest();
            incomingrequest.RequestBag.Keyword = text;

            return incomingrequest;
        }

        private static IncomingRequest getRequest()
        {
            List<String> names = new List<string>()
            {
                "msft",
                "aapl",
                "dash"
            };

            List<String> menmmomincs = new List<string>()
            {
                "PX_LAST",
                "SEUCRITY_TYPE",
                "BID",
                "ASK",
                "BID_SIZE",
                "ASK_SIZE"
            };

            IncomingRequest r = new IncomingRequest();
            r.CorrelationId = NewId.NextGuid().ToString();
            r.Datasource = "Factset";
            r.Requestor = "TestApp";
            r.UserId = "hali-lightpointft";
            r.Timestamp = DateTime.Now.ToString();
            r.Version = "1.01";


            List<SecurityDefinitionRequest> securities = new List<SecurityDefinitionRequest>();
            foreach (var sname in names)
            {
                securities.Add(new SecurityDefinitionRequest()
                {
                    IdentifierType = "TICKER",
                    SecurityIdentifier = sname
                });
            }

            r.RequestBag = new RequestBag();
            r.RequestBag.Securities.Add(securities);

            r.RequestBag.Fields.Add(menmmomincs.ToList());

            return r;

        }
    }
}
