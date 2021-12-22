using CommandProtocol.Transferable;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FactsetProcessor.proxy
{
    public class SearchAPIRequestModel
    {
        public Query query { get; set; }
        public Settings settings { get; set; }
    }

    public class Query
    {
        public string pattern { get; set; }
        public string[] entities { get; set; }
    }

    public class Settings
    {
        public int result_limit { get; set; }
    }

    public class SearchLookUp
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public async Task<SearchEventArgs> HitAPIAsync(string pattern, string[] entities, int result_limit, HttpClient client)
        {
            var query = new Query()
            {
                pattern = pattern,
                entities = entities
            };
            var settings = new Settings()
            {
                result_limit = result_limit
            };

            var RequestObject = new SearchAPIRequestModel()
            {
                query = query,
                settings = settings
            };


            var JsonObject = JsonConvert.SerializeObject(RequestObject);
            var stringContent = new StringContent(JsonObject.ToString());
            HttpResponseMessage result = null;
            SearchEventArgs Items = null;

            try
            {
                result = await client.PostAsync("https://api.factset.com/idsearch", stringContent);
                
                if (result.IsSuccessStatusCode)
                {
                    var jsonResult = result.Content.ReadAsStringAsync().Result;

                    Items = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchEventArgs>(jsonResult);

                    logger.Info("Search Results @{s}", new
                    {
                        Keyword = pattern,
                        ResultCount = Items.TypeaHead.ResultCount
                    });

                    if (logger.IsDebugEnabled)
                    {
                        foreach (var item in Items.TypeaHead.Results)
                        {
                            logger.Debug("Search Result @{r}", item.ToLogger());
                        }
                    }
                }
                else
                {
                    logger.Error($"HttpStatus code : {result.StatusCode} | Error response from factset in searchLookUp.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }


            

            return Items;
        }
    }
}
