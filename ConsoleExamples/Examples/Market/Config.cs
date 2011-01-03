// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
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
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Examples.Market
{
    public class Config
    {
        public const String DIRECTORY = "c:\\data\\";
        public const String STEP1 = Config.DIRECTORY + "step1_yahoo.csv";
        public const String STEP2 = Config.DIRECTORY + "step2_sort.csv";
        public const String STEP3 = Config.DIRECTORY + "step3_ind.csv";
        public const String STEP4 = Config.DIRECTORY + "step4_norm.csv";
        public const String STEP4STATS = Config.DIRECTORY + "step4_stats.csv";
        public const String STEP5 = Config.DIRECTORY + "step5_training.csv";
        public const String FILENAME_PREDICT = Config.DIRECTORY + "step5_predict.csv";
        public const String FILENAME = Config.DIRECTORY + "marketdata.eg";
        public const String MARKET_NETWORK = "market-network";
        public const String MARKET_TRAIN = "market-train";

        public const int TRAINING_MINUTES = 1;
        public const int HIDDEN1_COUNT = 20;
        public const int HIDDEN2_COUNT = 0;

        public const int TRAIN_BEGIN_YEAR = 2000;
        public const int TRAIN_BEGIN_MONTH = 1;
        public const int TRAIN_BEGIN_DAY = 1;

        public const int TRAIN_END_YEAR = 2008;
        public const int TRAIN_END_MONTH = 12;
        public const int TRAIN_END_DAY = 31;

        public const int INPUT_WINDOW = 10;
        public const int PREDICT_WINDOW = 1;
        public const String TICKER = "AAPL";
    }
}
