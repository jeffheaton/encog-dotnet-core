using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data.Market;

namespace Encog.Examples.Market
{
    public class Config
    {
        public static readonly String TRAINING_FILE = "marketData.egb";
        public static readonly String NETWORK_FILE = "marketNetwork.eg";
        public static readonly int TRAINING_MINUTES = 1;
        public static readonly int HIDDEN1_COUNT = 20;
        public static readonly int HIDDEN2_COUNT = 0;
        public static readonly int INPUT_WINDOW = 10;
        public static readonly int PREDICT_WINDOW = 1;
        public static readonly TickerSymbol TICKER = new TickerSymbol("AAPL");
    }
}
