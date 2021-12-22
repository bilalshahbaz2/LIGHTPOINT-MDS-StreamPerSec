using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace wsClientNetFramework
{
    class Program
    {
        static async Task Main(string[] args)
        {

            Console.WriteLine("Application started");
            Console.ReadLine();

            var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            //Set connection
            var connection = new HubConnectionBuilder().WithUrl("http://localhost:3623/hubs/collector")
                .WithAutomaticReconnect()
                .Build();

            await connection.StartAsync();


            _ = Task.Factory.StartNew(async () =>
            {
                var stream = connection.StreamAsync<OutgoingMessage>("FeedHandler", cancellationTokenSource.Token);
                var channel = await connection.StreamAsChannelAsync<OutgoingMessage>("FeedHandler", cancellationToken);
                while (await channel.WaitToReadAsync())
                {
                    OutgoingMessage message;
                    while (channel.TryRead(out message))
                    {
                        Console.WriteLine("Receved Message ");
                    }
                }
            });


            var tickers = new String[] { "DASH US EQUITY", "IBM US EQUITY" };

            await connection.SendAsync("Send", subscriptionRequest(tickers), cancellationToken);

            tickers = new String[] { "MSFT US EQUITY", "AAPL US EQUITY", "HOME US EQUITY" };

            await connection.SendAsync("Send", subscriptionRequest(tickers), cancellationToken);


            connection.On<KeepAliveMessage>("OnMessage", (message) =>
            {
                Console.WriteLine("FROM SERVER :: {0}", message);
            });



            Console.WriteLine("Press any Key top stop");
            Console.Read();
            cancellationTokenSource.Cancel();
            await connection.StopAsync();
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
            r.CorrelationId = Guid.NewGuid().ToString();
            r.Datasource = "Factset";
            r.Requestor = "TestApp";
            r.UserId = "hali-lightpointft";
            r.Timestamp = DateTime.Now.ToString();
            r.requestType = RequestType.Subscription;
            r.Version = "1.01";

            List<CommandProtocol.Requestable.SecurityDefinition> securities = new List<CommandProtocol.Requestable.SecurityDefinition>();
            foreach (var t in ticker)
            {
                securities.Add(new CommandProtocol.Requestable.SecurityDefinition()
                {
                    IdentifierType = "TICKER",
                    SecurityIdentifier = t
                });
            }

            r.RequestBag = new RequestBag();
            r.RequestBag.Securities.AddRange(securities.ToList());

            r.RequestBag.Fields.AddRange(menmmomincs.ToList());

            return r;
        }
    }
}
