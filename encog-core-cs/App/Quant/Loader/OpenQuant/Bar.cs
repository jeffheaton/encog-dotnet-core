//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Collections;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Encog.App.Quant.Loader.OpenQuant.Data
{
    /// <summary>
    ///     This holds the various data used by Openquant
    ///     Bars, and various custom series.
    /// </summary>
    public class Data
    {
        /// <summary>
        ///     Different data , a bar holds
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

        /// <summary>
        ///     Different possible prices on a bar.
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

        public enum BarSlycing
        {
            Normal,
            Equally
        }

        /// <summary>
        ///     The different bar types
        /// </summary>
        public enum BarType : byte
        {
            /// <summary>
            ///     Dynamic types.
            /// </summary>
            Dynamic = 5,

            /// <summary>
            ///     Range bars
            /// </summary>
            Range = 4,

            /// <summary>
            ///     Tick bars
            /// </summary>
            Tick = 2,

            /// <summary>
            ///     Time bars (open, high, low, close)
            /// </summary>
            Time = 1,
            Volume = 3
        }

        /// <summary>
        ///     Adds two numbers.
        /// </summary>
        private Func<int, int, int> Add = (x, y) => x + y;

        [Serializable]
        public class Bar : ICloneable
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="Bar" /> class.
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
            public Bar(BarType barType, long size, DateTime beginTime, DateTime endTime, double open, double high,
                       double low, double close, long volume, long openInt)
            {
                this.barType = barType;
                Size = size;
                BeginTime = beginTime;
                EndTime = endTime;
                Open = open;
                High = high;
                Low = low;
                Close = close;
                Volume = volume;
                OpenInt = openInt;
                ProviderId = 0;
                color = Color.Empty;
                IsComplete = false;
                DateKind = beginTime.Kind;
                DateTimeKind kind = beginTime.Kind;
                DateTimeKind kind2 = endTime.Kind;
            }


            /// <summary>
            ///     Initializes a new instance of the <see cref="Bar" /> class.
            /// </summary>
            /// <param name="size">The size.</param>
            /// <param name="open">The open.</param>
            /// <param name="high">The high.</param>
            /// <param name="low">The low.</param>
            /// <param name="close">The close.</param>
            public Bar(long size, double open, double high, double low, double close)
            {
                barType = barType;
                Size = size;
                BeginTime = beginTime;
                Open = open;
                High = high;
                Low = low;
                Close = close;
                ProviderId = 0;
                color = Color.Empty;
                IsComplete = false;
            }

            #region ICloneable Members

            public object Clone()
            {
                return MemberwiseClone();
            }

            #endregion

            public DateTime DateTime { get; set; }
            public double Close { get; set; }
            public double Open { get; set; }
            public double High { get; set; }
            public double Low { get; set; }
            public DateTime BeginTime { get; set; }
            protected DateTime EndTime { get; set; } //end time for this bar.
            public TimeSpan Duration { get; set; }
            // Fields
            protected BarType barType { get; set; }
            protected DateTime beginTime { get; set; } //Begin time for this bar
            protected Color color { get; set; } //Color the bar should be drawed
            protected bool IsComplete { get; set; } // is the bar complete.
            protected long OpenInt { get; set; } // open interests on othis bar.
            protected byte ProviderId { get; set; } // provider for this bar (a byte , e.g Simulated executions 1.
            protected long Size { get; set; } // the size : Lenght in seconds of this bar.
            protected long Volume { get; set; } // the volume of this bar.
            protected DateTimeKind DateKind { get; set; } //Bar kind.
            protected double Weighted { get; set; }
            protected double Median { get; set; }
            protected double Typical { get; set; }


            /// <summary>
            ///     Gets the last price for a bar price option
            /// </summary>
            /// <param name="option">The option.</param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.NoInlining)]
            public double GetPrice(BarPrice option)
            {
                switch (option)
                {
                    case BarPrice.High:
                        return High;

                    case BarPrice.Low:
                        return Low;

                    case BarPrice.Open:
                        return Open;

                    case BarPrice.Close:
                        return Close;

                    case BarPrice.Median:
                        return Median;

                    case BarPrice.Typical:
                        return Typical;

                    case BarPrice.Weighted:
                        return Weighted;
                }
                return 0.0;
            }
        }

        /// <summary>
        ///     holds arrays of bars
        /// </summary>
        public class BarArray : IEnumerable
        {
            // Methods

            public DataArray BarSeries;

            [MethodImpl(MethodImplOptions.NoInlining)]
            public BarArray()
            {
                BarSeries = new DataArray();
            }

            // Properties
            /// <summary>
            ///     Gets the <see cref="Bar" /> at the specified index.
            /// </summary>
            public Bar this[int index]
            {
                [MethodImpl(MethodImplOptions.NoInlining)] get { return this[index]; }
            }


            public IEnumerator GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <summary>
            ///     Adds the specified bar.
            /// </summary>
            /// <param name="bar">The bar.</param>
            public void Add(Bar bar)
            {
                BarSeries.Add(bar);
            }
        }

        public interface IDataObject
        {
            // Properties
            DateTime DateTime { get; set; }
            byte ProviderId { get; set; }
        }
    }
}
