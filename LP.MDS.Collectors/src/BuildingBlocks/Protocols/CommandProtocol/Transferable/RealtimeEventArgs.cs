using System;
using System.Collections.Generic;

namespace CommandProtocol.Transferable
{
    public class RealtimeEventArgs
    {
        public String correlationId { get; set; }
        public String ticker { get; set; }
        public Dictionary<String, String> fields { get; set; } = new Dictionary<string, string>();
    }
}
