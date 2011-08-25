using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Pattern;
using Encog.Util;
using Encog.Util.NetworkUtil;
using Encog.Util.Simple;
using SuperUtils = Encog.Util.NetworkUtil.NetworkUtility;
namespace Encog.Examples.RangeandMarket
{
    public static class CreateEval
    {




        #region create an evaluation set from a file and train it
        private static void CreateEvaluationSet(string @fileName)
        {
            List<double> Opens = SuperUtils.QuickParseCSV(fileName, "Open", 1200, 1200);
            List<double> High = NetworkUtility.QuickParseCSV(fileName, "High", 1200, 1200);
            List<double> Low = NetworkUtility.QuickParseCSV(fileName, "Low", 1200, 1200);
            List<double> Close = NetworkUtility.QuickParseCSV(fileName, "Close", 1200, 1200);
            List<double> Volume = NetworkUtility.QuickParseCSV(fileName, 5, 1200, 1200);
            TemporalMLDataSet superTemportal = new TemporalMLDataSet(100, 1);

            double[] Ranges = NetworkUtility.CalculateRanges(Opens.ToArray(), Close.ToArray());

            superTemportal = NetworkUtility.GenerateTrainingWithPercentChangeOnSerie(100, 1, Opens.ToArray(),
                                                                                                  Close.ToArray(), High.ToArray(), Low.ToArray(), Volume.ToArray());

            IMLDataPair aPairInput = SuperUtils.ProcessPair(NetworkUtility.CalculatePercents(Opens.ToArray()), NetworkUtility.CalculatePercents(Opens.ToArray()), 100, 1);
            IMLDataPair aPairInput3 = SuperUtils.ProcessPair(NetworkUtility.CalculatePercents(Close.ToArray()), NetworkUtility.CalculatePercents(Close.ToArray()), 100, 1);
            IMLDataPair aPairInput2 = SuperUtils.ProcessPair(NetworkUtility.CalculatePercents(High.ToArray()), NetworkUtility.CalculatePercents(High.ToArray()), 100, 1);
            IMLDataPair aPairInput4 = SuperUtils.ProcessPair(NetworkUtility.CalculatePercents(Volume.ToArray()), NetworkUtility.CalculatePercents(Volume.ToArray()), 100, 1);
            IMLDataPair aPairInput5 = SuperUtils.ProcessPair(NetworkUtility.CalculatePercents(Ranges.ToArray()), NetworkUtility.CalculatePercents(Ranges.ToArray()), 100, 1);
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

            Console.WriteLine("Percent Correct with normal Data Set:" + normalCorrectRate + " Percent Correct with temporal Dataset:" +
                      temporalErrorRate);





            Console.WriteLine("Paused , Press a key to continue to evaluation");
            Console.ReadKey();
        }
        public static double EvaluateNetworks(BasicNetwork network, BasicMLDataSet set)
        {
            int count = 0;
            int correct = 0;
            int CurrentIndex = 0;
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
                Console.WriteLine("Number" + "count" + @": actual=" + Format.FormatDouble(actual, 4) + @"(" + actualDirection + @")"
                                  + @",predict=" + Format.FormatDouble(predict, 4) + @"(" + predictDirection + @")" + @",diff=" + diff);
                CurrentIndex++;
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
            List<double> Opens = NetworkUtility.QuickParseCSV(fileName, "Open", startLine, HowMany);
            List<double> High = NetworkUtility.QuickParseCSV(fileName, "High", startLine, HowMany);
            List<double> Low = NetworkUtility.QuickParseCSV(fileName, "Low", startLine, HowMany);
            List<double> Close = NetworkUtility.QuickParseCSV(fileName, "Close", startLine, HowMany);
            List<double> Volume = NetworkUtility.QuickParseCSV(fileName, 5, startLine, HowMany);


            TemporalMLDataSet superTemportal = new TemporalMLDataSet(WindowSize, outputsize);

            double[] Ranges = NetworkUtility.CalculateRanges(Opens.ToArray(), Close.ToArray());




            superTemportal = NetworkUtility.GenerateTrainingWithPercentChangeOnSerie(100, 1, Opens.ToArray(),
                                                                                                  Close.ToArray(), High.ToArray(), Low.ToArray(), Volume.ToArray());

            IMLDataPair aPairInput = SuperUtils.ProcessPair(NetworkUtility.CalculatePercents(Opens.ToArray()), NetworkUtility.CalculatePercents(Opens.ToArray()), WindowSize, outputsize);
            IMLDataPair aPairInput3 = SuperUtils.ProcessPair(NetworkUtility.CalculatePercents(Close.ToArray()), NetworkUtility.CalculatePercents(Close.ToArray()), WindowSize, outputsize);
            IMLDataPair aPairInput2 = SuperUtils.ProcessPair(NetworkUtility.CalculatePercents(High.ToArray()), NetworkUtility.CalculatePercents(High.ToArray()), WindowSize, outputsize);
            IMLDataPair aPairInput4 = SuperUtils.ProcessPair(NetworkUtility.CalculatePercents(Volume.ToArray()), NetworkUtility.CalculatePercents(Volume.ToArray()), WindowSize, outputsize);
            IMLDataPair aPairInput5 = SuperUtils.ProcessPair(NetworkUtility.CalculatePercents(Ranges.ToArray()), NetworkUtility.CalculatePercents(Ranges.ToArray()), WindowSize, outputsize);
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
             List<double> Opens = NetworkUtility.QuickParseCSV(fileName, "Open", startLine, HowMany);
            List<double> High = NetworkUtility.QuickParseCSV(fileName, "High", startLine, HowMany);
            List<double> Low = NetworkUtility.QuickParseCSV(fileName, "Low", startLine, HowMany);
            List<double> Close = NetworkUtility.QuickParseCSV(fileName, "Close", startLine, HowMany);
            List<double> Volume = NetworkUtility.QuickParseCSV(fileName, 5, startLine, HowMany);
            TemporalMLDataSet superTemportal = new TemporalMLDataSet(WindowSize, outputsize);
            double[] Ranges = NetworkUtility.CalculateRanges(Opens.ToArray(), Close.ToArray());
            superTemportal = NetworkUtility.GenerateTrainingWithPercentChangeOnSerie(100, 1, Opens.ToArray(),
                                                                                                  Close.ToArray(), High.ToArray(), Low.ToArray(), Volume.ToArray());

            return superTemportal;
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
