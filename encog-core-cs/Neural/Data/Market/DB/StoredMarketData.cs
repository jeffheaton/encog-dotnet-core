using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData.Market.DB.Loader.YahooFinance;

namespace Encog.Neural.NeuralData.Market.DB
{
    [Serializable]
    public class StoredMarketData: IComparable<StoredMarketData>
    {
        private ulong date;
        private uint time;
        private double high;
        private double low;
        private double open;
        private double close;
        private ulong volume;

        /// <summary>
        /// The data that was collection for the sample date.
        /// </summary>
        private IDictionary<MarketDataType, object> data;

        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("EndOfDay: date=");
            result.Append(date);
            result.Append(", Close=");
            result.Append(close);
            return result.ToString();
        }

        public void Adjust(double adjust)
        {
            this.high *= adjust;
            this.low *= adjust;
            this.open *= adjust;
            this.close *= adjust;
        }

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

        public double High
        {
            get
            {
                return this.high;
            }
            set
            {
                this.high = value;
            }
        }

        public double Low
        {
            get
            {
                return this.low;
            }
            set
            {
                this.low = value;
            }
        }

        public double Open
        {
            get
            {
                return this.open;
            }
            set
            {
                this.open = value;
            }
        }

        public double Close
        {
            get
            {
                return this.close;
            }
            set
            {
                this.close = value;
            }
        }

        public ulong Volume
        {
            get
            {
                return this.volume;
            }
            set
            {
                this.volume = value;
            }
        }

        /// <summary>
        /// Set the specified type of data.
        /// </summary>
        /// <param name="t">The type of data to set.</param>
        /// <param name="d">The value to set.</param>
        public void SetData(MarketDataType t, double d)
        {
            switch (t)
            {
                case MarketDataType.OPEN:
                    Open = d;
                    break;
                case MarketDataType.CLOSE:
                    Close = d;
                    break;
                case MarketDataType.VOLUME:
                    Volume = (ulong)d;
                    break;
                case MarketDataType.HIGH:
                    High = d;
                    break;
                case MarketDataType.LOW:
                    Low = d;
                    break;
                default:
                    this.data[t] = d;
                    break;
            }
        }

        /// <summary>
        /// Set the specified type of data.
        /// </summary>
        /// <param name="t">The type of data to set.</param>
        /// <param name="d">The value to set.</param>
        public void SetData(MarketDataType t, long d)
        {
            this.data[t] = d;
        }

        /// <summary>
        /// Get the specified data type.
        /// </summary>
        /// <param name="t">The type of data to get.</param>
        /// <returns>The value.</returns>
        public double GetDataDouble(MarketDataType t)
        {
            switch(t)
            {
                case MarketDataType.OPEN:
                    return Open;
                case MarketDataType.CLOSE:
                    return Close;
                case MarketDataType.VOLUME:
                    return (double)Volume;
                case MarketDataType.HIGH:
                    return High;
                case MarketDataType.LOW:
                    return Low;
                default:
                    return (double)this.data[t];
            }
            
        }

        /// <summary>
        /// Get the specified data type.
        /// </summary>
        /// <param name="t">The type of data to get.</param>
        /// <returns>The value.</returns>
        public long GetDataLong(MarketDataType t)
        {
            switch (t)
            {
                case MarketDataType.OPEN:
                    return (long)Open;
                case MarketDataType.CLOSE:
                    return (long)Close;
                case MarketDataType.VOLUME:
                    return (long)Volume;
                case MarketDataType.HIGH:
                    return (long)High;
                case MarketDataType.LOW:
                    return (long)Low;
                default:
                    return (long)this.data[t];
            }
        }

        /// <summary>
        /// Compare this object with another of the same type.
        /// </summary>
        /// <param name="other">The other object to compare.</param>
        /// <returns>Zero if equal, greater or less than zero to indicate order.</returns>
        public int CompareTo(StoredMarketData other)
        {
            return this.Date.CompareTo(other.Date);
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
