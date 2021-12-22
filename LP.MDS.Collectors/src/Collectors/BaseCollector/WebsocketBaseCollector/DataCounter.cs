using CommandProtocol.Transferable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsocketBaseCollector
{
    public class DataCounter
    {
        private List<OutgoingMessage> secData;

        public DataCounter()
        {
            this.secData = new List<OutgoingMessage>();
        }

        public async Task AddMessage(OutgoingMessage message)
        {
            await Task.Run(() => this.secData.Add(message));
        }

        public async Task EmptyData()
        {
            await Task.Run(() => this.secData.Clear());
        }

        public async Task<int> GetCount()
        {
            return (await Task.Run(() =>  this.secData.Count ));
        }
    }
}
