using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data.Market;

namespace CSVanalyze
{
    class Config
    {
        public static readonly String TRAINING_FILE = "marketData.egb";
        public static readonly String NETWORK_FILE = "marketNetwork.eg";
        public const string DIRECTORY = @"c:\EncogOutput";
        public static readonly int TRAINING_MINUTES = 1;
        public static readonly int HIDDEN1_COUNT = 20;
        public static readonly int HIDDEN2_COUNT = 2;
        public static readonly int INPUT_WINDOW = 30;
        public static readonly int PREDICT_WINDOW = 1;
        public static readonly TickerSymbol TICKER = new TickerSymbol("EURUSD");
        public static readonly String ElhmanNetworkFile = "ElmhanNetwork.eg";
        public static readonly String ElmhanTrainingFile = "ElmhanTraining.egb";
        public static readonly String FeedNetworkFile = "FeedNetwork.eg";
        public static readonly String FeedTrainingFile = "FeedTraining.egb";
        public static readonly String GANetworkFile = "GANetwork.eg";
        public static readonly String GATrainingFile = "GATraining.egb";
        public static readonly String NeatNetworkFile = "NeatNetwork.eg";
        public static readonly String NeatTrainingFile = "NeatTraining.egb";

    }
}
