using System;
using System.Collections.Generic;
using System.Text;

namespace CommandProtocol.Transferable
{
    public class SearchEventArgs
    {
        public SearchResponseBag TypeaHead { get; set; }
    }

    public class SearchResponseBag
    {
        public ResultCount ResultCount { get; set; }
        public object Error { get; set; }
        public int IsSuccess { get; set; }
        public List<Result> Results { get; set; }
    }

    public class Result
    {
        public string Index { get; set; }
        public string Symbol { get; set; }
        public string Entity_id { get; set; }
        public string Name { get; set; }
        public string Ticker { get; set; }
        public string Fds_id { get; set; }

        public Object ToLogger() {
            return new { 
                Index =  this.Index,
                Symbol = this.Symbol,
                Entity_id = this.Entity_id,
                Name = this.Name,
                Ticker = this.Ticker,
                Fds_id = this.Fds_id
            };
        
        }
    }

    public class ResultCount
    {
        public int Equities { get; set; }
    }
}
