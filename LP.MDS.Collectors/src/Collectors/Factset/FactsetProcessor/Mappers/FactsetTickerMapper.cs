using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactsetProcessor.Mappers
{
    public interface IFactsetTickerMapper {

        List<string> toFactSet(List<string> tickers);

        string toSourceCollector(string ticker);
        string toFactSet(string securityIdentifier);
    }

    public class FactsetTickerMapper : IFactsetTickerMapper
    {
        private Dictionary<string, string> mapping = new Dictionary<string, string> { 
            { "AAON US Equity", "AAON-US" },
            { "BEN US Equity", "BEN-US" },
            { "EL US Equity", "EL-US" }, 
            { "CHEF US Equity", "CHEF-US" }, 
            { "AAPL US Equity", "AAPL-US" }, 
            { "AAPL Equity", "AAPL-US" }, 
            { "MSFT US Equity", "MSFT-US" }, 
            { "NIO US Equity", "NIO-US" }, 
            { "IBM US Equity", "IBM-US" }, 
            { "CRM US Equity", "CRM-US" }, 
            { "JPM US Equity", "JPM-US" }, 
            { "FB US Equity", "FB-US" }, 
            { "FB EQUITY", "FB-US" }, 
            { "AMZN US Equity", "AMZN-US" } };

        public FactsetTickerMapper()
        {

        }

        public List<string> toFactSet(List<string> tickers)
        {
            List<string> mappedTickers = new List<string>();
            foreach (var item in tickers)
            {
                if (mapping.ContainsKey(item))
                {
                    mappedTickers.Add(mapping.GetValueOrDefault(item));
                }
                else
                {
                    mappedTickers.Add(item);
                }

            }
            return mappedTickers;
        }

        public string toFactSet(string securityIdentifier)
        {
            string key = null;
            mapping.TryGetValue(securityIdentifier, out key);

            return key ?? securityIdentifier;
        }

        public string toSourceCollector(string ticker)
        {
            string key = "";
            foreach (var item in mapping)
            {
                if (item.Value == ticker)
                {
                    key = item.Key;
                    break;
                }
            }
            return key;
        }
    }
}
