using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandProtocol.Requestable;
using FluentValidation;
using NLog;
using ProcessorProtocol;
using ServiceProtocol.Validators;

namespace ServiceProtocol.Services
{
    public interface SubscriptionManager
    {
        void Add(IncomingRequest incomingRequest);
        Task RemoveByConnectionId(String connectionId);

        void RegisterConnection(String ConnectionId);

        List<String> GetSubscriptions(String ticker);
        int TotalConnectedClient();
        int TotalSubscribedTickers();

        IncomingRequest FindByCorrelationId(String CorrelationId);

        bool Exists(string correlationId);


    }

    public class SubscriptionManagerImpl : SubscriptionManager
    {

        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private ConcurrentDictionary<String, ConcurrentDictionary<String, String>> subscriptionCache;
        private ConcurrentDictionary<String, List<String>> tickerSubsciptionCache;
        private ConcurrentDictionary<String, List<String>> correlationTickersCache;
        private ConcurrentDictionary<String, IncomingRequest> incomingRequestCache;

        private ConcurrentDictionary<String, List<String>> connectionRequestCache;

        public SubscriptionManagerImpl()
        {
            this.incomingRequestCache = new ConcurrentDictionary<string, IncomingRequest>();
            this.tickerSubsciptionCache = new ConcurrentDictionary<string, List<string>>();
            this.subscriptionCache = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
            this.connectionRequestCache = new ConcurrentDictionary<string, List<string>>();
            this.correlationTickersCache = new ConcurrentDictionary<string, List<string>>();

        }

        private void Add(string correlationId, string ticker)
        {
            if (!subscriptionCache.ContainsKey(correlationId))
            {
                ConcurrentDictionary<String, String> emptyTickers = new ConcurrentDictionary<string, string>();
                this.subscriptionCache.TryAdd(correlationId, emptyTickers);
            }



            ConcurrentDictionary<String, String> correlateionIdTickers;
            this.subscriptionCache.TryGetValue(correlationId, out correlateionIdTickers);
            correlateionIdTickers.TryAdd(ticker, ticker);



            if (!tickerSubsciptionCache.ContainsKey(ticker))
            {
                tickerSubsciptionCache.TryAdd(ticker, new List<string>());
            }

            List<String> tickerSubscirptions;
            tickerSubsciptionCache.TryGetValue(ticker, out tickerSubscirptions);
            tickerSubscirptions.Add(correlationId);
        }

        public void Add(string correlationId, string[] tickers)
        {
            foreach (var ticker in tickers)
            {
                this.Add(correlationId, ticker);
            }
            this.correlationTickersCache.TryAdd(correlationId, tickers.ToList());
        }

        public void Add(IncomingRequest incomingRequest)
        {
            //
            if (!this.incomingRequestCache.ContainsKey(incomingRequest.CorrelationId))
            {
                this.incomingRequestCache.TryAdd(incomingRequest.CorrelationId, incomingRequest);

                string[] tickers = incomingRequest.RequestBag.Securities.Select(s => s.SecurityIdentifier).ToArray();

                this.Add(incomingRequest.CorrelationId, tickers);


            }



        }

        public IncomingRequest FindByCorrelationId(string CorrelationId)
        {
            if (this.incomingRequestCache.ContainsKey(CorrelationId))
            {
                return this.incomingRequestCache[CorrelationId];
            }
            return null;
        }

        public List<string> GetSubscriptions(string ticker)
        {

            if (!tickerSubsciptionCache.ContainsKey(ticker))
            {
                return new List<string>();
            }
            List<String> tickerSubscirptions;
            tickerSubsciptionCache.TryGetValue(ticker, out tickerSubscirptions);

            logger.Debug($"Total of {tickerSubscirptions.Count} found for ticker {ticker}");
            return tickerSubscirptions;
        }

        public bool Exists(string correlationId)
        {
            return this.incomingRequestCache.ContainsKey(correlationId);
        }

        public async Task RemoveByConnectionId(string connectionId)
        {
            await Task.Run(() =>
            {
                if (this.connectionRequestCache.ContainsKey(connectionId))
                {

                    ConcurrentDictionary<String, ConcurrentDictionary<String, String>> subscribeCache = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>(subscriptionCache);
                    ConcurrentDictionary<String, List<String>> tickerSubscibeCache = new ConcurrentDictionary<string, List<string>>(tickerSubsciptionCache);
                    ConcurrentDictionary<String, List<String>> correlationTickerCache = new ConcurrentDictionary<string, List<string>>(correlationTickersCache);
                    ConcurrentDictionary<String, IncomingRequest> incomingReqCache = new ConcurrentDictionary<string, IncomingRequest>(incomingRequestCache);
                    ConcurrentDictionary<String, List<String>> connectionReqCache = new ConcurrentDictionary<string, List<string>>(connectionRequestCache);

                    var disconnected = incomingReqCache.Where(x => x.Value.ConnectionId.Equals(connectionId)).ToList();
                    var correlationIds = disconnected.Select(x => x.Value.CorrelationId).ToList();

                    foreach (var id in correlationIds)
                    {
                        this.subscriptionCache.TryRemove(subscribeCache.FirstOrDefault(x => x.Key.Equals(id)));
                        this.incomingRequestCache.TryRemove(incomingReqCache.FirstOrDefault(x => x.Key.Equals(id)));

                        //var obj = this.correlationTickersCache.FirstOrDefault(x => x.Key.Contains(id));

                        foreach (var item in tickerSubscibeCache)
                        {
                            var newList = new List<string>();
                            var oldList = tickerSubscibeCache[item.Key];

                            newList = oldList.Where(x => x != id).ToList();
                            this.tickerSubsciptionCache[item.Key] = newList;
                        }
                        
                    }

                    this.connectionRequestCache.TryRemove(connectionReqCache.FirstOrDefault(x => x.Key.Equals(connectionId)));
                }
            });

        }

        public void RegisterConnection(string connectionId)
        {
            if (!String.IsNullOrEmpty(connectionId))
            {
                this.connectionRequestCache.TryAdd(connectionId, new List<string>() {
                connectionId
                });
            }

        }

        public int TotalConnectedClient()
        {
            ConcurrentDictionary<String, List<String>> connectionReqCache = new ConcurrentDictionary<String, List<String>>(connectionRequestCache);
            List<string> totalconnectedClients = new List<string>();
            totalconnectedClients = connectionReqCache.Select(kvp => kvp.Key).ToList();
            var clients = totalconnectedClients.Count();
            return clients;
        }

        public int TotalSubscribedTickers()
        {

            ConcurrentDictionary<String, List<String>> subsciptionTickerCache = new ConcurrentDictionary<String, List<String>>(tickerSubsciptionCache);
            List<string> tickers = new List<string>();
            tickers = subsciptionTickerCache.Select(kvp => kvp.Key).ToList();
            var Count = 0;

            foreach (var i in tickers)
            {
                Count = Count + subsciptionTickerCache[i].Count();
            }

            return Count;
        }



    }

}
