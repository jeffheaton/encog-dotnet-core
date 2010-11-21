using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData.Market.DB.Loader.YahooFinance;

namespace Encog.Neural.NeuralData.Market.DB
{
    [Serializable]
    public class StoredAdjustmentData
    {
        private ulong date;
        private uint time;
        private uint numerator;
        private uint denominator;
        private double div;
        private double adjustment;

        public void CalculateAdjustment(double close)
        {
            if (numerator > 0 && denominator > 0)
            {
                adjustment = (double)denominator / (double)numerator;
            }
            else
            {
                adjustment = 1.0 - (this.div / close);
            }
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            if (numerator > 0 && denominator > 0)
            {
                result.Append("Split: date=");
                result.Append(date);
                result.Append(",num=");
                result.Append(numerator);
                result.Append(",den=");
                result.Append(denominator);
                result.Append(",adj=");
                result.Append(adjustment);
            }
            else
            {
                result.Append("Div: date=");
                result.Append(date);
                result.Append(",amt=");
                result.Append(div);
                result.Append(",adj=");
                result.Append(adjustment);
            }
            return result.ToString();
        }

        public bool IsDividend()
        {
            return this.numerator == 0 && this.denominator == 0;
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

        public uint Numerator
        {
            get
            {
                return this.numerator;
            }
            set
            {
                this.numerator = value;
            }
        }

        public uint Denominator
        {
            get
            {
                return this.denominator;
            }
            set
            {
                this.denominator = value;
            }
        }

        public double Div
        {
            get
            {
                return this.div;
            }
            set
            {
                this.div = value;
            }
        }

        public double Adjustment
        {
            get
            {
                return this.adjustment;
            }
            set
            {
                this.adjustment = value;
            }
        }
    }
}
