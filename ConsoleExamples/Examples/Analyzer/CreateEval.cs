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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Temporal;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Pattern;
using Encog.Util;
using Encog.Util.NetworkUtil;
using Encog.Util.Simple;

namespace Encog.Examples.Analyzer
{
    public static class CreateEval
    {




        #region create an evaluation set from a file and train it
// ReSharper disable UnusedMember.Local
        private static void CreateEvaluationSet(string @fileName)
// ReSharper restore UnusedMember.Local
        {
            List<double> Opens = QuickCSVUtils.QuickParseCSV(fileName, "Open", 1200, 1200);
            List<double> High = QuickCSVUtils.QuickParseCSV(fileName, "High", 1200, 1200);
            List<double> Low = QuickCSVUtils.QuickParseCSV(fileName, "Low", 1200, 1200);
            List<double> Close = QuickCSVUtils.QuickParseCSV(fileName, "Close", 1200, 1200);
            List<double> Volume = QuickCSVUtils.QuickParseCSV(fileName, 5, 1200, 1200);
         

            double[] Ranges = NetworkUtility.CalculateRanges(Opens.ToArray(), Close.ToArray());

            TemporalMLDataSet superTemportal = TrainerHelper.GenerateTrainingWithPercentChangeOnSerie(100, 1, Opens.ToArray(),
                                                                                                  Close.ToArray(), High.ToArray(), Low.ToArray(), Volume.ToArray());

            IMLDataPair aPairInput = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(Opens.ToArray()), NetworkUtility.CalculatePercents(Opens.ToArray()), 100, 1);
            IMLDataPair aPairInput3 = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(Close.ToArray()), NetworkUtility.CalculatePercents(Close.ToArray()), 100, 1);
            IMLDataPair aPairInput2 = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(High.ToArray()), NetworkUtility.CalculatePercents(High.ToArray()), 100, 1);
            IMLDataPair aPairInput4 = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(Volume.ToArray()), NetworkUtility.CalculatePercents(Volume.ToArray()), 100, 1);
            IMLDataPair aPairInput5 = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(Ranges.ToArray()), NetworkUtility.CalculatePercents(Ranges.ToArray()), 100, 1);
            List<IMLDataPair> listData = new List<IMLDataPair>();
            listData.Add(aPairInput);
            listData.Add(aPairInput2);
            listData.Add(aPairInput3);
            listData.Add(aPairInput4);
            listData.Add((aPairInput5));


            var minitrainning = new BasicMLDataSet(listData);

            var network = (BasicNetwork)CreateElmanNetwork(100, 1);
            double normalCorrectRate = EvaluateNetworks(network, minitrainning);

            double temporalErrorRate = EvaluateNetworks(network, superTemportal);

            Console.WriteLine(@"Percent Correct with normal Data Set:" + normalCorrectRate + @" Percent Correct with temporal Dataset:" +
                      temporalErrorRate);





            Console.WriteLine(@"Paused , Press a key to continue to evaluation");
            Console.ReadKey();
        }
        public static double EvaluateNetworks(BasicNetwork network, BasicMLDataSet set)
        {
            int count = 0;
            int correct = 0;
            foreach (IMLDataPair pair in set)
            {
                IMLData input = pair.Input;
                IMLData actualData = pair.Ideal;
                IMLData predictData = network.Compute(input);

                double actual = actualData[0];
                double predict = predictData[0];
                double diff = Math.Abs(predict - actual);

               Direction  actualDirection = DetermineDirection(actual);
               Direction predictDirection = DetermineDirection(predict);

                if (actualDirection == predictDirection)
                    correct++;
                count++;
                Console.WriteLine(@"Number" + @"count" + @": actual=" + Format.FormatDouble(actual, 4) + @"(" + actualDirection + @")"
                                  + @",predict=" + Format.FormatDouble(predict, 4) + @"(" + predictDirection + @")" + @",diff=" + diff);
               
            }
            double percent = correct / (double)count;
            Console.WriteLine(@"Direction correct:" + correct + @"/" + count);
            Console.WriteLine(@"Directional Accuracy:"
                              + Format.FormatPercent(percent));

            return percent;
        }
        #endregion

        #region Direction enum

        public enum Direction
        {
            Up,
            Down
        } ;



        public static Direction DetermineDirection(double d)
        {
            return d < 0 ? Direction.Down : Direction.Up;
        }

        #endregion

        public static double TrainNetworks(BasicNetwork network, IMLDataSet minis)
        {
            Backpropagation trainMain = new Backpropagation(network, minis,0.0001,0.6);
            //set the number of threads below.
            trainMain.ThreadCount = 0;
            // train the neural network
            ICalculateScore score = new TrainingSetScore(minis);
            IMLTrain trainAlt = new NeuralSimulatedAnnealing(network, score, 10, 2, 100);
           // IMLTrain trainMain = new Backpropagation(network, minis, 0.0001, 0.01);
            
            StopTrainingStrategy stop = new StopTrainingStrategy(0.0001, 200);
            trainMain.AddStrategy(new Greedy());
            trainMain.AddStrategy(new HybridStrategy(trainAlt));
            trainMain.AddStrategy(stop);

            //prune strategy not in GIT!...Removing it.
            //PruneStrategy strategypruning = new PruneStrategy(0.91d, 0.001d, 10, network,minis, 0, 20);
            //trainMain.AddStrategy(strategypruning);

            EncogUtility.TrainConsole(trainMain,network,minis, 15.2);


            var sw = new Stopwatch();
            sw.Start();
            while (!stop.ShouldStop())
            {
                trainMain.Iteration();
                
                Console.WriteLine(@"Iteration #:" + trainMain.IterationNumber + @" Error:" + trainMain.Error + @" Genetic Iteration:" + trainAlt.IterationNumber);
            }
            sw.Stop();
            Console.WriteLine(@"Total elapsed time in seconds:" + TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).Seconds);

            return trainMain.Error;
        }

        public static BasicMLDataSet CreateEvaluationSetAndLoad(string @fileName, int startLine, int HowMany, int WindowSize, int outputsize)
        {
            List<double> Opens = QuickCSVUtils.QuickParseCSV(fileName, "Open", startLine, HowMany);
            List<double> High = QuickCSVUtils.QuickParseCSV(fileName, "High", startLine, HowMany);
            // List<double> Low = QuickCSVUtils.QuickParseCSV(fileName, "Low", startLine, HowMany);
            List<double> Close = QuickCSVUtils.QuickParseCSV(fileName, "Close", startLine, HowMany);
            List<double> Volume = QuickCSVUtils.QuickParseCSV(fileName, 5, startLine, HowMany);
            double[] Ranges = NetworkUtility.CalculateRanges(Opens.ToArray(), Close.ToArray());
            IMLDataPair aPairInput = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(Opens.ToArray()), NetworkUtility.CalculatePercents(Opens.ToArray()), WindowSize, outputsize);
            IMLDataPair aPairInput3 = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(Close.ToArray()), NetworkUtility.CalculatePercents(Close.ToArray()), WindowSize, outputsize);
            IMLDataPair aPairInput2 = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(High.ToArray()), NetworkUtility.CalculatePercents(High.ToArray()), WindowSize, outputsize);
            IMLDataPair aPairInput4 = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(Volume.ToArray()), NetworkUtility.CalculatePercents(Volume.ToArray()), WindowSize, outputsize);
            IMLDataPair aPairInput5 = TrainerHelper.ProcessPairs(NetworkUtility.CalculatePercents(Ranges.ToArray()), NetworkUtility.CalculatePercents(Ranges.ToArray()), WindowSize, outputsize);
            List<IMLDataPair> listData = new List<IMLDataPair>();
            listData.Add(aPairInput);
            listData.Add(aPairInput2);
            listData.Add(aPairInput3);
            listData.Add(aPairInput4);
            listData.Add((aPairInput5));


            var minitrainning = new BasicMLDataSet(listData);
            return minitrainning;
        }

        public static TemporalMLDataSet GenerateATemporalSet(string @fileName, int startLine, int HowMany, int WindowSize, int outputsize)
        {
            List<double> Opens = QuickCSVUtils.QuickParseCSV(fileName, "Open", startLine, HowMany);
            List<double> High = QuickCSVUtils.QuickParseCSV(fileName, "High", startLine, HowMany);
            List<double> Low = QuickCSVUtils.QuickParseCSV(fileName, "Low", startLine, HowMany);
            List<double> Close = QuickCSVUtils.QuickParseCSV(fileName, "Close", startLine, HowMany);
            List<double> Volume = QuickCSVUtils.QuickParseCSV(fileName, 5, startLine, HowMany);

            return TrainerHelper.GenerateTrainingWithPercentChangeOnSerie(WindowSize, outputsize, Opens.ToArray(), Close.ToArray(), High.ToArray(), Low.ToArray(), Volume.ToArray());
        }


        public static IMLMethod CreateElmanNetwork(int inputsize, int outputsize)
        {
            // construct an Elman type network
            ElmanPattern pattern = new ElmanPattern();

            pattern.ActivationFunction = new ActivationTANH();
            pattern.InputNeurons = inputsize;
            
            pattern.AddHiddenLayer(0);
            pattern.OutputNeurons = outputsize;
            return pattern.Generate();
            
        }

        


    }
}
