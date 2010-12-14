using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.MarketDB;

namespace Encog.App.Quant.Stats
{
    public class MarketStats
    {
        private MarketDataStorage storage;

        public double High { get; set; }
        public double Low { get; set; }
        public double AverageClose { get; set; }
        public double AverageVolume { get; set; }
        public ulong LowVolume { get; set; }
        public ulong HighVolume { get; set; }
        public ulong Count { get; set; }

        public MarketStats(MarketDataStorage storage)
        {
            this.storage = storage;
        }

        public void Calculate(String ticker, DateTime current, BarPeriod period)
        {
            DateTime from = current.AddYears(-1);
            Calculate(ticker, from, current, period);
        }

        public void Calculate(String ticker, DateTime from, DateTime to, BarPeriod period)
        {
            IList<StoredMarketData> list = storage.LoadRange(ticker, from, to, period);
            this.High = double.MinValue;
            this.Low = double.MaxValue;
            ulong vol = 0;
            double close = 0;

            ulong count = 0;
            foreach( StoredMarketData data in list)
            {
                this.High = Math.Max(this.High, data.Close);
                this.Low = Math.Min(this.Low, data.Close);
                this.HighVolume = Math.Max(this.HighVolume, data.Volume);
                this.LowVolume = Math.Min(this.LowVolume, data.Volume);
                close += data.Close;
                count++;
                vol += data.Volume;
            }

            if (count == 0)
            {
                this.AverageVolume = 0;
                this.AverageClose = 0;
            }
            else
            {
                this.AverageVolume = vol / count;
                this.AverageClose = close / count;
            }

            this.Count = count;

        }
    }
}
