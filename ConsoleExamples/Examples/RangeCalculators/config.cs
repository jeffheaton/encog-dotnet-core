using System;
using Encog.ML.Data.Market;

namespace Encog.Examples.RangeCalculators
{
    public class RangeConfig
    {
        public static readonly String TRAINING_FILE = "RangemarketData.egb";
        public static readonly String NETWORK_FILE = "RangemarketNetwork.eg";
        public static readonly String DIRECTORY = @"c:\EncogOutput";
        public static readonly int TRAINING_MINUTES = 1;
        public static readonly int HIDDEN1_COUNT = 20;
        public static readonly int HIDDEN2_COUNT;
        public static readonly int INPUT_WINDOW = 30;
        public static readonly int PREDICT_WINDOW = 3;
        public static readonly TickerSymbol TICKER = new TickerSymbol("EURUSD");
    }
}