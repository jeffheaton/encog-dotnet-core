using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData.Market.DB;
using Encog.Neural.NeuralData.Market.DB.Loader.YahooFinance;

namespace Encog.Neural.Data.Market.DB
{
    [Serializable]
    public class StoredData
    {
        private ulong date;
        private uint time;

        public DateTime Date
        {
            get
            {
                if (this.date == 0)
                    return YahooDownload.EARLIEST_DATE;
                else
                    return DateUtil.Long2DateTime(this.date);
            }
            set
            {
                this.date = DateUtil.DateTime2Long(value);
            }
        }

        public DateTime Time
        {
            get
            {
                if (this.time == 0)
                    return YahooDownload.EARLIEST_DATE;
                else
                    return DateUtil.Int2Time(this.Date, this.time);
            }
            set
            {
                this.time = DateUtil.Time2Int(value);
            }
        }

        public ulong EncodedDate
        {
            get
            {
                return this.date;
            }
            set
            {
                this.date = value;
            }
        }

        public uint EncodedTime
        {
            get
            {
                return this.time;
            }
            set
            {
                this.time = value;
            }
        }
    }
}
