using FactsetProcessor.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactsetProcessorClient.Services
{
    public class NoTickerMapper : IFactsetTickerMapper
    {
        public List<string> toFactSet(List<string> tickers)
        {
            return tickers;
        }

        public string toFactSet(string securityIdentifier)
        {
            return securityIdentifier;
        }

        public string toSourceCollector(string ticker)
        {
            return ticker;
        }
    }
}
