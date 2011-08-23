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
using Encog.Util.Normalize.Segregate.Index;
using Encog.Util.Normalize.Target;
using Encog.Util.Simple;
using SuperUtils = Encog.Util.NetworkUtil.NetworkUtility;
namespace ConsoleSuperTrainer
{
    class Program : IStatusReportable
    {
        private const string file = "c:\\EURUSD_Hourly_Bid_1999.03.04_2011.03.14.csv";
        private const string GoldFile = "XAUUSD_Hourly_Bid_2007.08.23_2011.08.23.csv";


        static void Main(string[] args)
        {


            DataNormalization norming = new DataNormalization();
            if (args.Length > 0)
            {
                if (args[0].ToLowerInvariant() == "normalize")
                {

                    Step1(GoldFile, CONFIG.DataFileDirectory);
                   // InsertVolumeInCSV(GoldFile);
                    //NormalizeAndStore(GoldFile, norming);
                }
                if (args[0].ToLowerInvariant() == "train")
                {
                    //Train the network.

                }
            }

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
            //first we put all our inputs in the normalization.
            _norm.Report = new ConsoleStatusReportable();
            _norm.AddInputField(Open = new InputFieldCSV(true, CONFIG.DataFileDirectory+fileUsed, "Open"));
            _norm.AddInputField(High = new InputFieldCSV(true, CONFIG.DataFileDirectory + fileUsed, "High"));
            _norm.AddInputField(Low = new InputFieldCSV(true, CONFIG.DataFileDirectory + fileUsed, "Low"));
            _norm.AddInputField(Close = new InputFieldCSV(true, CONFIG.DataFileDirectory + fileUsed, "Close"));
            _norm.AddInputField(Volume = new InputFieldCSV(true, CONFIG.DataFileDirectory + fileUsed,"Volume"));
            //Lets calculate the ranges so we can also use them in the file.
            //We put our output field.
            _norm.AddOutputField(new OutputFieldRangeMapped(Open));
            _norm.AddOutputField(new OutputFieldRangeMapped(High));
            _norm.AddOutputField(new OutputFieldRangeMapped(Low));
            _norm.AddOutputField(new OutputFieldRangeMapped(Close));
            _norm.AddOutputField(new OutputFieldRangeMapped(Volume));

            _norm.Storage = new NormalizationStorageCSV(CONFIG.DIRECTORY+ fileUsed.Substring(0,6)+ CONFIG.NormalizedFile);
            _norm.CSVFormatUsed = CSVFormat.English;
            _norm.Process(true);
            SuperUtils.SaveNormalization(CONFIG.DIRECTORY, CONFIG.Normalization, _norm);

        }


        public static void Step1(string afile, string directory)
        {
            Console.WriteLine(@"Step 1: Generate training and evaluation files");
            Console.WriteLine(@"Generate training file");
            Copy(new FileInfo(CONFIG.DataFileDirectory + afile), new FileInfo(CONFIG.DIRECTORY+ CONFIG.NormalizedFile), 0, 2, 4); // take 3/4
            Console.WriteLine(@"Generate evaluation file");
            Copy(new FileInfo(CONFIG.DataFileDirectory + afile), new FileInfo(CONFIG.DIRECTORY + CONFIG.EvaluationFile), 3, 3, 4); // take 1/4
        }
        public static void Copy(FileInfo source, FileInfo target, int start, int stop, int size)
        {
            var inputField = new IInputField[5];

            var norm = new DataNormalization {Storage = new NormalizationStorageCSV(target.ToString()) };
            for (int i = 1; i < 5; i++)
            {
                inputField[i] = new InputFieldCSV(true, source.ToString(), i);
                norm.AddInputField(inputField[i]);
                IOutputField outputField = new OutputFieldDirect(inputField[i]);
                norm.AddOutputField(outputField);
            }

            // load only the part we actually want, i.e. training or eval
            var segregator2 = new IndexSampleSegregator(start, stop, size);
            norm.AddSegregator(segregator2);

            norm.Process();
        }

        public static void InsertVolumeInCSV(string afile)
        {

            FileInfo utilFile = new FileInfo(CONFIG.DataFileDirectory+afile);

          //  FileInfo moddedFile = FileUtil.AddFilenameBase(utilFile, "_Moded");

            //FileInfo moddedFile = utilFile.


            //StreamReader rd = new StreamReader(utilFile.ToString());
            // string headers =  rd.ReadLine();

            StreamWriter wter = new StreamWriter(afile);
           
            wter.WriteLine("Date,Open,High,Low,Close,Volume");
            wter.Close();
            //headers = rd.ReadLine();
            //rd.Close();


        }

        #region IStatusReportable Members

        public void Report(int total, int current, String message)
        {
            Console.WriteLine(current + @"/" + total + @" " + message);
        }

        #endregion
    }

    public static class CONFIG
    {
        public const string DIRECTORY = @"C:\EncogOutput\";
        public static string Normalization = "NormalizedNets.ega";
        public static string NormalizedFile = "File_Normalization.eg";
        public static string DataFileDirectory = @"C:\Datas\";
        public static string EvaluationFile = "EValFile_Normalized.eg";
    }
}
