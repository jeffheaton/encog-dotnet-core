using System;
using Encog.ML.Data.Market;

namespace Encog.Examples.SVMPredictCSV
{
    public static class CONFIG
    {
        public static readonly String TRAINING_FILE = "SVMmarketData.egb";
        public static readonly String SVMTRAINING_FILE = "SVM_marketData.egb";
        public static readonly String NETWORK_FILE = "Elmhan_marketNetwork.eg";
        public static readonly String SVMNETWORK_FILE = "SVM_marketNetwork.eg";
        public static readonly int TRAINING_MINUTES = 1;
        public static readonly int HIDDEN1_COUNT = 20;
        public static readonly int HIDDEN2_COUNT = 3;
        public static readonly int INPUT_WINDOW = 100;
        public static readonly int PREDICT_WINDOW = 1;
        public static readonly TickerSymbol TICKER = new TickerSymbol("EURUSD");
        public const string DIRECTORY = @"C:\EncogOutput\";

        public static int StartLine = 2;
        public static int HowManyLines = 500;

        public static int STARTING_YEAR = 1;
        public static int WINDOW_SIZE = 30;
        public static int TRAIN_START = WINDOW_SIZE;
        public static int TRAIN_END = 500;
        public static int EVALUATE_START = 501;
        public static int EVALUATE_END;
    }
}