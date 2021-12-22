using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebsocketBaseCollector
{
    public class TimeCounter
    {
        private int currentSec;
        private Timer timer;
        private bool flag;
        private DataCounter _secData;

        public TimeCounter(DataCounter secData)
        {
            this._secData = secData;
            this.currentSec = 1;
            this.flag = true;
        }

        public async Task Increase()
        {
            await Task.Run(() => this.currentSec++);
        }

        public async Task startTimer()
        {
            await Task.Run(() =>
            {
                if (flag)
                {
                    flag = false;
                    this.timer = new Timer(minCount, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
                }
            });
        }

        private void minCount(object state)
        {
            //Console.WriteLine($"{this.currentSec}--{this.minData.Count}");
            //this.minData.Clear();
            //currentSec++;

            Console.WriteLine($"{this.currentSec}--{this._secData.GetCount().Result}");
            Task.Run(() => this._secData.EmptyData());
            currentSec++;
        }
    }
}
