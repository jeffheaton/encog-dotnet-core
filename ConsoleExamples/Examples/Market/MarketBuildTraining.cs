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
using Encog.Util.Logging;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Persist;
using System.IO;
using Encog.App.Quant.MarketDB;
using Encog.App.Quant.Loader.YahooFinance;
using Encog.App.Quant.Dataset;
using Encog.Util.CSV;
using Encog.App.Quant.Sort;
using Encog.App.Quant.Indicators;
using Encog.App.Quant.Normalize;
using Encog.App.Quant.Temporal;

namespace Encog.Examples.Market
{
    public class MarketBuildTraining
    {
        public void Run()
        {
            Console.WriteLine("Downloading market data");
            Logging.StopConsoleLogging();

            DateTime begin = new DateTime(
                Config.TRAIN_BEGIN_YEAR, 
                Config.TRAIN_BEGIN_MONTH, 
                Config.TRAIN_BEGIN_DAY);

            DateTime end = new DateTime(
                Config.TRAIN_END_YEAR,
                Config.TRAIN_END_MONTH,
                Config.TRAIN_END_DAY);

            YahooDownload loader = new YahooDownload();
            //loader.LoadAllData("aapl","d:\\data\\aapl.csv",CSVFormat.ENGLISH,begin,end);
            SortCSV sort = new SortCSV();
            sort.SortOrder.Add(new SortedField(0,SortType.SortInteger,true));
            sort.Process("d:\\data\\aapl.csv","d:\\data\\aapl2.csv",true,CSVFormat.ENGLISH);
            ProcessIndicators indicators = new ProcessIndicators();
            indicators.Analyze("d:\\data\\aapl2.csv", true, CSVFormat.ENGLISH);
            indicators.Columns[0].Output = false;
            indicators.Columns[1].Output = false;
            indicators.Columns[2].Output = false;
            indicators.Columns[3].Output = false;
            indicators.Columns[4].Output = false;
            indicators.Columns[5].Output = false;
            indicators.Columns[6].Output = false;
            indicators.Columns[7].Output = false;
            indicators.RenameColumn(5, FileData.CLOSE);
            indicators.AddColumn(new MovingAverage(30, true));
            indicators.Process("d:\\data\\aapl3.csv");

            EncogNormalize normalize = new EncogNormalize();
            normalize.Analyze("d:\\data\\aapl3.csv", true, CSVFormat.ENGLISH);
            normalize.Normalize("d:\\data\\aapl4.csv");
            normalize.WriteStatsFile("d:\\data\\aapl.norm");

            TemporalWindow window = new TemporalWindow();
            window.Analyze("d:\\data\\aapl4.csv", true, CSVFormat.ENGLISH);
            window.InputWindow = 30;
            window.PredictWindow = 1;
            window.Fields[0].Input = true;
            window.Fields[0].Predict = true;
            window.Process("d:\\data\\aapl5.csv");

            Console.WriteLine("Building training data");
        }
    }
}
