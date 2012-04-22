//
// Encog(tm) Core v3.1 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2012 Heaton Research, Inc.
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
