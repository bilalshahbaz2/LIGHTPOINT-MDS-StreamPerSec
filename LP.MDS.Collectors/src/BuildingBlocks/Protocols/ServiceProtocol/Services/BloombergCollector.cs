using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Services
{
    public class BloombergCollector 
    {
        private List<string> collectorFields;

        public BloombergCollector()
        {
            collectorFields = new List<string>() { "SECURITY_NAME", "TICKER", "BASE_CRNCY", "EXCH_CODE" };
        }

        public List<string> GetFields()
        {
            return new List<string>(collectorFields);
        }
    }
}
