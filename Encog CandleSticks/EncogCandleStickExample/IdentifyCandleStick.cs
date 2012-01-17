// Encog Simple Candlestick Example
// Copyright 2010 by Jeff Heaton (http://www.jeffheaton.com)
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data.Market;
using Encog.ML.Data.Market.Loader;


namespace EncogCandleStickExample
{
    public class IdentifyCandleStick
    {

        public const double EQUAL_PRECISION = 0.01;

        public const int UNKNOWN = -1;
        public const int BLACK_CANDLE = 0;
        public const int DOJI = 1;
        public const int DOJI_DRAGONFLY = 2;
        public const int DOJI_GRAVESTONE = 3;
        public const int DOJI_LONGSHADOW = 4;
        public const int HAMMER = 5;
        public const int HAMMER_INVERTED = 6;
        public const int LONG_LOWER = 7;
        public const int LONG_UPPER = 8;
        public const int MARUBOZU_BLACK = 9;
        public const int MARUBOZU_WHITE = 10;
        public const int TOP_BLACK = 11;
        public const int TOP_WHITE = 12;
        public const int WHITE_CANDLE = 13;

        private double open;
        private double close;
        private double high;
        private double low;
        private double bodyTop;
        private double bodyBottom;

        public void SetStats(LoadedMarketData data)
        {
            this.open = data.GetData(MarketDataType.Open);
            this.close = data.GetData(MarketDataType.Close);
            this.high = data.GetData(MarketDataType.High);
            this.low = data.GetData(MarketDataType.Low);
            this.bodyTop = Math.Max(this.open, this.close);
            this.bodyBottom = Math.Min(this.open, this.close);
        }

        public int DeterminePattern()
        {
            if (this.IsMarubozuBlack())
            {
                return IdentifyCandleStick.MARUBOZU_BLACK;
            }
            else if (this.IsMarubozuWhite())
            {
                return IdentifyCandleStick.MARUBOZU_WHITE;
            }
            else if (IsLongUpper())
            {
                return IdentifyCandleStick.LONG_UPPER;
            }
            else if (IsLongLower())
            {
                return IdentifyCandleStick.LONG_LOWER;
            }
            else if (IsTopWhite())
            {
                return IdentifyCandleStick.TOP_WHITE;
            }
            else if (IsTopBlack())
            {
                return IdentifyCandleStick.TOP_BLACK;
            }
            else if (IsWhiteCandle())
            {
                return IdentifyCandleStick.WHITE_CANDLE;
            }
            else if (IsBlackCandle())
            {
                return IdentifyCandleStick.BLACK_CANDLE;
            }
            else if (this.IsDojiLongShadow())
            {
                return IdentifyCandleStick.DOJI_LONGSHADOW;
            }
            else if (this.IsDojiGraveStone())
            {
                return IdentifyCandleStick.DOJI_GRAVESTONE;
            }
            else if (this.IsDojiDragonfly())
            {
                return IdentifyCandleStick.DOJI_DRAGONFLY;
            }
            else if (this.IsHammer())
            {
                return IdentifyCandleStick.HAMMER;
            }
            else if (this.IsInvertedHammer())
            {
                return IdentifyCandleStick.HAMMER_INVERTED;
            }
            return IdentifyCandleStick.UNKNOWN;
        }

        public bool IsBlackCandle()
        {
            return (HasBody() &&
                    HasUpperShadow() &&
                    HasLowerShadow() &&
                    IsBlack());
        }

        public bool IsWhiteCandle()
        {
            return (HasBody() &&
                    HasUpperShadow() &&
                    HasLowerShadow() &&
                    IsBlack());
        }

        public bool IsDoji()
        {
            return (!HasBody() &&
                    HasUpperShadow() &&
                    HasLowerShadow());
        }

        public bool IsDojiGraveStone()
        {
            return (!HasBody() &&
                    HasUpperShadow() &&
                    !HasLowerShadow());
        }

        public bool IsDojiDragonfly()
        {
            return (!HasBody() &&
                    !HasUpperShadow() &&
                    HasLowerShadow());
        }

        public bool IsDojiLongShadow()
        {
            return (!HasBody() &&
                    HasLongLowerShadow() &&
                    HasLongUpperShadow());
        }

        public bool IsHammer()
        {
            return (IsWhite() &&
                    HasSmallBody() &&
                    !HasUpperShadow() &&
                    HasLowerShadow());
        }

        public bool IsInvertedHammer()
        {
            return (IsBlack() &&
                    HasSmallBody() &&
                    HasUpperShadow() &&
                    !HasLowerShadow());
        }

        public bool IsLongLower()
        {
            return (IsWhite() &&
                    HasLongLowerShadow() &&
                    !HasLongUpperShadow() &&
                    HasBody() &&
                    !HasSmallBody() &&
                    HasUpperShadow() &&
                    HasLowerShadow());
        }

        public bool IsLongUpper()
        {
            return (IsBlack() &&
                    !HasLongLowerShadow() &&
                    HasLongUpperShadow() &&
                    HasBody() &&
                    !HasSmallBody() &&
                    HasUpperShadow() &&
                    HasLowerShadow());
        }

        public bool IsMarubozuWhite()
        {
            return (IsWhite() &&
                    HasBody() &&
                    !HasLowerShadow() &&
                    !HasUpperShadow());
        }

        public bool IsMarubozuBlack()
        {
            return (IsBlack() &&
                    HasBody() &&
                    !HasLowerShadow() &&
                    !HasUpperShadow());
        }

        public bool IsTopBlack()
        {
            return IsBlackCandle() && HasSmallBody();
        }

        public bool IsTopWhite()
        {
            return IsWhiteCandle() && HasSmallBody();
        }


        public bool HasSmallBody()
        {
            return (Math.Abs(this.open - this.close) < 1.0);
        }

        public bool HasLowerShadow()
        {
            return ((this.bodyBottom - this.low) > EQUAL_PRECISION);
        }

        public bool HasUpperShadow()
        {
            return ((this.high - this.bodyTop) > EQUAL_PRECISION);
        }

        public bool HasBody()
        {
            return (Math.Abs(this.open - this.close) > IdentifyCandleStick.EQUAL_PRECISION);
        }

        public bool IsWhite()
        {
            return this.close > this.open;
        }

        public bool IsBlack()
        {
            return !IsWhite();
        }

        public double LowerShadowLength
        {
            get
            {
                return this.high - this.bodyTop;
            }
        }

        public double UpperShadowLength
        {
            get
            {
                return this.bodyBottom - this.low;
            }
        }

        public bool HasLongLowerShadow()
        {
            return (LowerShadowLength > (UpperShadowLength * 2));
        }

        public bool HasLongUpperShadow()
        {
            return (UpperShadowLength > (UpperShadowLength * 2));
        }
    }
}
