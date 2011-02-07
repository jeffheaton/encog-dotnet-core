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
using Encog.App.Quant.Loader.YahooFinance;
using Encog.Util.CSV;
using Encog.App.Quant.Sort;
using Encog.App.Quant.Indicators;
using Encog.App.Quant.Normalize;
using Encog.App.Quant.Temporal;
using Encog.Neural.Data.Buffer;
using Encog.Neural.Data.Buffer.CODEC;
using Encog.Neural.NeuralData;
using Encog.Util.Simple;
using Encog.Neural.Data.Basic;
using Encog.Persist.Location;
using Encog.Engine.Network.Activation;
using Encog.App.Quant.Basic;

namespace Encog.Examples.Market
{
    public class MarketBuildTraining
    {
        public static void Generate(DateTime begin, DateTime end, bool forTraining)
        {
            Console.WriteLine("Downloading market data");
            Logging.StopConsoleLogging();

            YahooDownload loader = new YahooDownload();
            loader.LoadAllData(Config.TICKER,Config.STEP1,CSVFormat.ENGLISH,begin,end);

            if (forTraining)
            {
                Console.WriteLine("Building training data");
            }
            else
            {
                Console.WriteLine("Building evaluation data");
            }

            // sort the downloaded market data
            SortCSV sort = new SortCSV();
            sort.SortOrder.Add(new SortedField(0,SortType.SortInteger,true));
            sort.Process(Config.STEP1, Config.STEP2, true, CSVFormat.ENGLISH);

            // calculate moving average
            ProcessIndicators indicators = new ProcessIndicators();
            indicators.Analyze(Config.STEP2, true, CSVFormat.ENGLISH);
            indicators.Columns[0].Output = true;
            indicators.Columns[1].Output = false;
            indicators.Columns[2].Output = false;
            indicators.Columns[3].Output = false;
            indicators.Columns[4].Output = false;
            indicators.Columns[5].Output = false;
            indicators.Columns[6].Output = false;
            indicators.Columns[7].Output = false;
            indicators.RenameColumn(5, FileData.CLOSE);
            indicators.AddColumn(new MovingAverage(30, true));
            indicators.Process(Config.STEP3);

            // normalize
            if (forTraining)
            {
                NormalizeCSV normalize = new NormalizeCSV();
                normalize.Analyze(Config.STEP3, true, CSVFormat.ENGLISH);
                normalize.Stats[0].Action = NormalizationDesired.PassThrough;
                normalize.Normalize(Config.STEP4);
                normalize.WriteStatsFile(Config.STEP4STATS);
            }
            else
            {
                NormalizeCSV normalize = new NormalizeCSV();
                normalize.SetSourceFile(Config.STEP3, true, CSVFormat.ENGLISH);
                normalize.ReadStatsFile(Config.STEP4STATS);
                normalize.Normalize(Config.STEP4);
            }

            // build temporal training data
            TemporalWindowCSV window = new TemporalWindowCSV();
            window.Analyze(Config.STEP4, true, CSVFormat.ENGLISH);
            window.InputWindow = Config.INPUT_WINDOW;
            window.PredictWindow = Config.PREDICT_WINDOW;
            window.Fields[0].Action = forTraining ? TemporalType.Ignore : TemporalType.PassThrough;
            window.Fields[1].Action = TemporalType.InputAndPredict;

            if (forTraining)
            {
                Console.WriteLine("Generating training data");
                window.Process(Config.STEP5);
            }
            else
            {
                Console.WriteLine("Generating prediction data");
                window.Process(Config.FILENAME_PREDICT);
            }

            Console.WriteLine("Done processing data.");

        }

        public void Run()
        {
            DateTime begin = new DateTime(
                Config.TRAIN_BEGIN_YEAR,
                Config.TRAIN_BEGIN_MONTH,
                Config.TRAIN_BEGIN_DAY);

            DateTime end = new DateTime(
                Config.TRAIN_END_YEAR,
                Config.TRAIN_END_MONTH,
                Config.TRAIN_END_DAY);

            Generate(begin, end, true);

            INeuralDataSet training = (BasicNeuralDataSet)EncogUtility.LoadCSV2Memory(Config.STEP5, Config.INPUT_WINDOW, Config.PREDICT_WINDOW, true, CSVFormat.ENGLISH);

            // build a neural network
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(new ActivationTANH(), true, Config.INPUT_WINDOW));
            network.AddLayer(new BasicLayer(new ActivationTANH(), true, Config.HIDDEN1_COUNT));
            network.AddLayer(new BasicLayer(new ActivationLinear(), true, Config.PREDICT_WINDOW));
            network.Structure.FinalizeStructure();
            network.Reset();

            EncogMemoryCollection encog = new EncogMemoryCollection();
            encog.Add(Config.MARKET_NETWORK, network);
            
            encog.Add(Config.MARKET_TRAIN, (IEncogPersistedObject)training);
            encog.Save(Config.FILENAME);
        }
    }
}
