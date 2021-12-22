using CommandProtocol.Extenstions;
using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using FactsetProcessor.Mappers;
using FactsetProcessor.proxy;
using gRPCFactset;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FactsetProcessor.V2.FactsetProxy
{
    public interface IFactsetProxy
    {
        List<Dictionary<string, string>> GetRefDataAsync(IncomingRequest incomingRequest);

        Task<SearchEventArgs> GetSearchDataAsync(IncomingRequest incomingRequest);
    }

    public class FactsetProxy : IFactsetProxy
    {
        private static ILogger logger = NLog.LogManager.GetCurrentClassLogger();


        private readonly FactsetConfiguration factsetConfiguration;
        private readonly IFactsetTickerMapper tickerMapper;

        public FactsetProxy(FactsetConfiguration _factsetConfiguration, IFactsetTickerMapper _tickerMapper)
        {
            factsetConfiguration = _factsetConfiguration;
            tickerMapper = _tickerMapper;
        }

        public List<Dictionary<string, string>> GetRefDataAsync(IncomingRequest request)
        {
            List<string> tickers = request.RequestBag.Securities.Where(item => item != null && !String.IsNullOrEmpty(item.SecurityIdentifier)).Select(item => item.SecurityIdentifier).ToList();
            if (tickers == null || tickers.Count == 0)
            {
                logger.Warn("No Valid Ticker Request is found @{r}", request.ToLogger());
            }

            return GetRefDataAsync(tickers);
        }

        public async Task<SearchEventArgs> GetSearchDataAsync(IncomingRequest incomingRequest)
        {
            var text = incomingRequest.RequestBag.Keyword;

            var result = await CallAPI(text);

            return result;
        }

        private async Task<SearchEventArgs> CallAPI(string text)
        {
            var userName = factsetConfiguration.ApiKeyUser;
            var password = factsetConfiguration.ApiKeyPassword;
            var authToken = Encoding.ASCII.GetBytes($"{userName}:{password}");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                var arr = new string[] { "equities" };

                SearchLookUp SearchAPIService = new SearchLookUp();
                var result = await SearchAPIService.HitAPIAsync(text, arr, 20, client);

                return result;
            }
        }

        private List<Dictionary<string, string>> GetRefDataAsync(List<string> tickers)
        {
            List<Dictionary<string, string>> valuePairs = new List<Dictionary<string, string>>();
            List<string> factsetTickers = new List<string>();

            var userName = factsetConfiguration.Username;
            var password = factsetConfiguration.Password;
            var authToken = Encoding.ASCII.GetBytes($"{userName}:{password}");

            using (var client = new HttpClient())
            {

                factsetTickers = tickerMapper.toFactSet(tickers);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                var url = "https://datadirect.factset.com/services/DataFetch";

                NameValueCollection queryStringParams = System.Web.HttpUtility.ParseQueryString(string.Empty);

                queryStringParams.Add("id", String.Join(",", factsetTickers));
                queryStringParams.Add("format", "json");
                queryStringParams.Add("report", "SEC_REF_DD_ADV_LP");

                var requestURL = $"{url}?{queryStringParams.ToString()}";
                logger.Info("Request URL @{url}", new { url = requestURL });
                var requestHandler = client.GetAsync(requestURL).Result;

                if(requestHandler.IsSuccessStatusCode)
                {
                    var result = requestHandler.Content.ReadAsStringAsync().Result;

                    valuePairs = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(result);

                    return valuePairs;
                }

                return null;
            }
        }
    }
}
