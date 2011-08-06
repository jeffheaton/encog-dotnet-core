using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
#pragma warning disable 1591
namespace Encog.App.Quant.Loader.OpenQuant.Data
{
    /// <summary>
    /// This holds the various data used by Openquant 
    /// Bars, and various custom series.
    /// </summary>
    public class Data
    {

        [Serializable]
        public class Bar : ICloneable
        {
            public DateTime DateTime { get; set; }
            public double Close { get; set; }
            public double Open { get; set; }
#pragma warning disable 1591
            public double High { get; set; }

            public double Low { get; set; }
            public DateTime BeginTime { get; set; }
            protected DateTime EndTime { get; set; }//end time for this bar.
            public TimeSpan Duration { get; set; }
            // Fields
            protected BarType barType { get; set; }
            protected DateTime beginTime { get; set; }   //Begin time for this bar
            protected Color color { get; set; }//Color the bar should be drawed
            protected bool IsComplete { get; set; } // is the bar complete.
            protected long OpenInt { get; set; } // open interests on othis bar.
            protected byte ProviderId { get; set; } // provider for this bar (a byte , e.g Simulated executions 1.
            protected long Size { get; set; } // the size : Lenght in seconds of this bar.
            protected long Volume { get; set; } // the volume of this bar.
            protected DateTimeKind DateKind { get; set; } //Bar kind.
            protected double Weighted { get; set; }
            protected double Median { get; set; }
            protected double Typical { get; set;}


            /// <summary>
            /// Gets the last price for a bar price option
            /// </summary>
            /// <param name="option">The option.</param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.NoInlining)]
            public double GetPrice(BarPrice option)
            {
                switch (option)
                {
                    case BarPrice.High:
                        return this.High;

                    case BarPrice.Low:
                        return this.Low;

                    case BarPrice.Open:
                        return this.Open;

                    case BarPrice.Close:
                        return this.Close;

                    case BarPrice.Median:
                        return this.Median;

                    case BarPrice.Typical:
                        return this.Typical;

                    case BarPrice.Weighted:
                        return this.Weighted;
                }
                return 0.0;
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="Bar"/> class.
            /// </summary>
            /// <param name="barType">Type of the bar.</param>
            /// <param name="size">The size.</param>
            /// <param name="beginTime">The begin time.</param>
            /// <param name="endTime">The end time.</param>
            /// <param name="open">The open.</param>
            /// <param name="high">The high.</param>
            /// <param name="low">The low.</param>
            /// <param name="close">The close.</param>
            /// <param name="volume">The volume.</param>
            /// <param name="openInt">The open int.</param>
            public Bar(BarType barType, long size, DateTime beginTime, DateTime endTime, double open, double high, double low, double close, long volume, long openInt)
            {

                this.barType = barType;
                this.Size = size;
                this.BeginTime = beginTime;
                this.EndTime = endTime;
                this.Open = open;
                this.High = high;
                this.Low = low;
                this.Close = close;
                this.Volume = volume;
                this.OpenInt = openInt;
                this.ProviderId = 0;
                this.color = Color.Empty;
                this.IsComplete = false;
                this.DateKind = beginTime.Kind;
                DateTimeKind kind = beginTime.Kind;
                DateTimeKind kind2 = endTime.Kind;
            }



            /// <summary>
            /// Initializes a new instance of the <see cref="Bar"/> class.
            /// </summary>
            /// <param name="size">The size.</param>
            /// <param name="open">The open.</param>
            /// <param name="high">The high.</param>
            /// <param name="low">The low.</param>
            /// <param name="close">The close.</param>
            public Bar(long size, double open, double high, double low, double close)
            {

                this.barType = barType;
                this.Size = size;
                this.BeginTime = beginTime;
                this.Open = open;
                this.High = high;
                this.Low = low;
                this.Close = close;
                this.ProviderId = 0;
                this.color = Color.Empty;
                this.IsComplete = false;

            }
            #region ICloneable Members

            public object Clone()
            {
                return MemberwiseClone();
            }

            #endregion


        }



        /// <summary>
        /// Adds two numbers.
        /// </summary>
        Func<int, int, int> Add = (x, y) => x + y;

        /// <summary>
        /// holds arrays of bars
        /// </summary>
        public class BarArray : IEnumerable
        {
            // Methods
            [MethodImpl(MethodImplOptions.NoInlining)]
            public BarArray()
            {
                BarSeries = new DataArray();

            }

            public DataArray BarSeries;

            // Properties
            /// <summary>
            /// Gets the <see cref="Bar"/> at the specified index.
            /// </summary>
            public Bar this[int index]
            {
                [MethodImpl(MethodImplOptions.NoInlining)]
                get
                {
                    return this[index];
                }
            }


            /// <summary>
            /// Adds the specified bar.
            /// </summary>
            /// <param name="bar">The bar.</param>
            public void Add(Bar bar)
            {
                this.BarSeries.Add((Bar) bar);

            }
            public IEnumerator GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }





        public enum BarSlycing
        {
            Normal,
            Equally
        }
        /// <summary>
        /// The different bar types
        /// </summary>
        public enum BarType : byte
        {
            /// <summary>
            /// Dynamic types.
            /// </summary>
            Dynamic = 5,

            /// <summary>
            /// Range bars
            /// </summary>
            Range = 4,
            /// <summary>
            /// Tick bars
            /// </summary>
            Tick = 2,
            /// <summary>
            /// Time bars (open, high, low, close)
            /// </summary>
            Time = 1,
            Volume = 3
        }

        /// <summary>
        /// Different possible prices on a bar.
        /// </summary>
        public enum BarPrice
        {
            High,
            Low,
            Open,
            Close,
            Median,
            Typical,
            Weighted
        }

        public interface IDataObject
        {
            // Properties
            DateTime DateTime { get; set; }
            byte ProviderId { get; set; }
        }


        /// <summary>
        /// Different data , a bar holds
        /// </summary>
        public enum BarData
        {
            Close,
            Open,
            High,
            Low,
            Median,
            Typical,
            Weighted,
            Volume,
            OpenInt
        }

 


    }

}

#pragma warning restore 1591