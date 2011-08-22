using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Encog;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Util;
using Encog.Util.CSV;
using Encog.Util.File;
using Encog.Util.Normalize;
using Encog.Util.Normalize.Input;
using Encog.Util.Normalize.Output;
using Encog.Util.Normalize.Target;
using Encog.Util.Simple;

namespace ConsoleSuperTrainer
{
    class Program
    {
        private const string file = "c:\\EURUSD_Hourly_Bid_1999.03.04_2011.03.14.csv";
        static void Main(string[] args)
        {

            DataNormalization norming = new DataNormalization();

            NormalizeAndStore(file, norming);


        }




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

                Direction actualDirection = DetermineDirection(actual);
                Direction predictDirection = DetermineDirection(predict);

                if (actualDirection == predictDirection)
                    correct++;
                count++;
                Console.WriteLine(@"Day " + count + @":actual="
                                  + Format.FormatDouble(actual, 4) + @"(" + actualDirection + @")"
                                  + @",predict=" + Format.FormatDouble(predict, 4) + @"("
                                  + predictDirection + @")" + @",diff=" + diff);
            }
            double percent = correct / (double)count;
            Console.WriteLine(@"Direction correct:" + correct + @"/" + count);
            Console.WriteLine(@"Directional Accuracy:"
                              + Format.FormatPercent(percent));

            return percent;
        }



        public static double TrainNetworks(BasicNetwork network, IMLDataSet minis)
        {
            // train the neural network
            ICalculateScore score = new TrainingSetScore(minis);
            IMLTrain trainAlt = new NeuralSimulatedAnnealing(
                network, score, 10, 2, 100);
            IMLTrain trainMain = new Backpropagation(network, minis, 0.00001, 0.0);
            StopTrainingStrategy stop = new StopTrainingStrategy();
            trainMain.AddStrategy(new Greedy());
            // trainMain.AddStrategy(new HybridStrategy(trainAlt));
            trainMain.AddStrategy(stop);
            // IMLTrain train = EncogUtility.TrainConsole(network, superTemportal);

            var sw = new Stopwatch();
            sw.Start();
            // run epoch of learning procedure
            for (int i = 0; i < 3000; i++)
            {
                trainMain.Iteration();
                Console.WriteLine("Iteration #:" + trainMain.IterationNumber + " Error:" + trainMain.Error);
            }
            sw.Stop();

            return trainMain.Error;
        }



        public static void NormalizeAndStore(string fileUsed, DataNormalization _norm )
        {
            IInputField ifield;
             IInputField Open;
             IInputField High;
             IInputField Low;
             IInputField Close;
             IInputField Volume;

            // create arrays to hold the normalized sunspots
            double [][] FirstArray = new double[5][];
            double[] First = new double[6];
            FileInfo infoFile = new FileInfo(fileUsed);
            FileInfo CopiedFile = infoFile.CopyTo("c:\\NewEuro.csv",true);

          

          

            ////we remove the headers.
            //StreamReader rd = new StreamReader(Environment.CurrentDirectory+CopiedFile);
            //rd.ReadLine();
            //StreamWriter wt = new StreamWriter(Environment.CurrentDirectory + CopiedFile);
            //wt.Write(rd);
            //wt.Close();
            //rd.Close();
            //headers removed , now we can keep going.

            // normalize the sunspots
            _norm.CSVFormatUsed = CSVFormat.English;
            //first we put all our inputs in the normalization.
            _norm.Report = new ConsoleStatusReportable();
            _norm.AddInputField(Open = new InputFieldCSV(true, fileUsed, "Open"));


            _norm.AddInputField(High = new InputFieldCSV(true,fileUsed,"High"));
            _norm.AddInputField(Low = new InputFieldCSV(true, fileUsed, "Low"));
            _norm.AddInputField(Close = new InputFieldCSV(true, fileUsed,"Close"));
            _norm.AddInputField(Volume = new InputFieldCSV(true, fileUsed, "Volume"));
            //We put our output field.
            _norm.AddOutputField(new OutputFieldRangeMapped(Close));

            _norm.Storage = new NormalizationStorageCSV("c:\\NewEuro.csv");
            _norm.CSVFormatUsed = CSVFormat.English;
            _norm.Process(true);


        }
        
    }
}
