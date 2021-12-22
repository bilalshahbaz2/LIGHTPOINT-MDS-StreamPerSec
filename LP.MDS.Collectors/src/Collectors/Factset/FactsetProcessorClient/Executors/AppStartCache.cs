using CommandProtocol.Requestable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactsetProcessorClient.Executors
{
    public class AppStartCache
    {
        static AppStartCache()
        {
            CachedResults = new Dictionary<string, object>();
        }

        public static Dictionary<string, object> CachedResults { get; set; }

        public static void CacheData(string key, object value)
        {
            CachedResults.Add(key, value);
        }

        public static void PurgeCache(string key)
        {
            CachedResults.Remove(key);
        }

        //public static Tuple<bool, object> GetCachedData(string key)
        //return new Tuple<bool, object>(exists, data);
        public static Tuple<IncomingRequest> GetCachedData(string key)
        {
            object data;
            var exists = CachedResults.TryGetValue(key, out data);
            return new Tuple<IncomingRequest>((IncomingRequest)data);
        }
    }
}
