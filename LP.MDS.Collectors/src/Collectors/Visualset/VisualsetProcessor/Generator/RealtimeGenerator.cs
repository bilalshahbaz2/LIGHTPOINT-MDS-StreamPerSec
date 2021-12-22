using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using CommandProtocol.Transferable;
using Microsoft.Extensions.Hosting;
using NLog;
using ProcessorProtocol;

namespace VisualsetProcessor.Generator
{
    public class RealtimeItem
    {
        public String correlationId { get; set; }
        public String ticker { get; set; }
    }

    public class RealtimeItemMnemonics
    {
        public String correlationId { get; set; }
        public String ticker { get; set; }
        public List<String> mnemonics { get; set; }

        public RealtimeItemMnemonics()
        {
            this.mnemonics = new List<string>();
        }
    }


    public class RealtimeGenerator : IHostedService
    {
        private readonly List<String> tickers;
        private readonly List<String> mnemonics;

        private static ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly Faker faker;
        private readonly BlockingCollection<RealtimeItem> realtimeFrequencyQueue;
        private readonly BlockingCollection<RealtimeItemMnemonics> realtimeItemQueue;

        private readonly MarketDataProcessor marketDataProcessor;

        public RealtimeGenerator(MarketDataProcessor marketDataProcessor)
        {
            this.tickers = new List<string>();
            this.mnemonics = new List<string>();
            Randomizer.Seed = new Random(8675309);
            this.faker = new Faker();
            this.Init();
            this.realtimeFrequencyQueue = new BlockingCollection<RealtimeItem>(new ConcurrentQueue<RealtimeItem>());
            this.realtimeItemQueue = new BlockingCollection<RealtimeItemMnemonics>(new ConcurrentQueue<RealtimeItemMnemonics>());
            this.marketDataProcessor = marketDataProcessor;
        }

        private void Init()
        {
            string[] ticketsGenerator = new string[] { "IBM", "DASH", "APPL", "MSFT", "HOME"};
            foreach(var t in ticketsGenerator)
            {
                this.tickers.Add( $"{t} US EQUITY");
                for( int start = 1; start< 0; start++)
                {
                    var paddedStart = $"{start}".PadLeft(2, '0');
                    this.tickers.Add($"{t}-{paddedStart} US EQUITY");
                }
            }


            string[] _m = new string[] { "BID", "ASK", "BID_SIZE", "ASK_SIZE", "FUT_VAL_PT", "PX_LAST" };
            this.mnemonics.AddRange( _m );
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.Info("Started SusbcriptionGenerator Service ");

            this.FrequencyGenerator();
            this.RealTimeIterationProcessor();
            return Task.CompletedTask;
        }


        private void FrequencyGenerator()
        {
            Thread t1 = new Thread(() => {
                while (true)
                {
                    int frequencyIterationCount = this.faker.Random.Int(0, this.tickers.Count);
                    logger.Info($"Generating {frequencyIterationCount} tickers for this Iteration");
                    var realtimeTickers = this.faker.PickRandom(this.tickers, frequencyIterationCount);
                    foreach( var itemTicker in realtimeTickers)
                    {
                        this.realtimeFrequencyQueue.Add(new RealtimeItem()
                        {
                            ticker = itemTicker
                        });
                    }
                    Thread.Sleep(2 * 1000);
                }
            });
            t1.Start();

            Thread realtimeFrequencyConsumer = new Thread(() => {
                while (true)
                {
                    var realtimeItem = this.realtimeFrequencyQueue.Take();
                    List<String> itemMnemonics =  this.faker.PickRandom(this.mnemonics, 4).ToList();

                    logger.Info("Pushing into ItemMenmonics Collection");
                    this.realtimeItemQueue.Add(new RealtimeItemMnemonics()
                    {
                        ticker = realtimeItem.ticker,
                        mnemonics = itemMnemonics
                    });
                }
            });
            realtimeFrequencyConsumer.Start();
        }


        private void RealTimeIterationProcessor()
        {
            Thread iterationThread = new Thread( () => {
                while (true)
                {
                    RealtimeItemMnemonics ItemMnemonics = this.realtimeItemQueue.Take();
                    RealtimeEventArgs itemEvent = new RealtimeEventArgs();
                    foreach( var mnemonic in ItemMnemonics.mnemonics)
                    {
                        itemEvent.fields.Add(mnemonic, this.randomValue(mnemonic).ToString() ) ;
                    }
                    itemEvent.ticker = ItemMnemonics.ticker;
                    itemEvent.correlationId = ItemMnemonics.correlationId;


                    marketDataProcessor.HandleEvent( itemEvent );

                    logger.Info($"itemEvent Generated {itemEvent.ticker}");
                }
            });
            iterationThread.Start();
        }


        private Decimal randomValue(String menmonicName)
        {
            return this.faker.Random.Decimal(0, Decimal.Parse("1.95"));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            return Task.CompletedTask;
        }
    }
}
