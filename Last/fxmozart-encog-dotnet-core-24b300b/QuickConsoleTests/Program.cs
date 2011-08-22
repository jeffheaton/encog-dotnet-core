using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.MathUtil;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Market;
using Encog.ML.Data.Temporal;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.NeuralData;
using Encog.Neural.Pattern;
using Encog.Persist;
using Encog.Util;
using Encog.Util.NetworkUtil;
using Encog.Util.Simple;
using SuperUtility =  Encog.Util.NetworkUtil;

namespace QuickConsoleTests
{
    class Program
    {
        static void Main(string[] args)
        {

            SuperTemporalVersusBasic();
            ////make inputs.
           // RandomTrainer();

            Console.ReadKey();
            return ;
        }


        private static void RandomTrainer()
        {
            double[] firstinput = MakeInputs(3000);
            double[] SecondInput = MakeInputs(3000);
            double[] ThirdInputs = MakeInputs(3000);
            double[] FourthInputs = MakeInputs(3000);
            var pair = ProcessPairs(firstinput, FourthInputs, 3000, 20);
            var pair2 = ProcessPairs(SecondInput, FourthInputs, 3000, 20);
            var pair3 = ProcessPairs(ThirdInputs, FourthInputs, 3000, 20);
            var pair4 = ProcessPairs(FourthInputs, FourthInputs, 3000, 20);
            BasicMLDataSet SuperSet = new BasicMLDataSet();
            SuperSet.Add(pair);
            SuperSet.Add(pair2);
            SuperSet.Add(pair3);
            SuperSet.Add(pair4);
            var network = (BasicNetwork)CreateElmanNetwork(SuperSet.InputSize, SuperSet.IdealSize);
            double error = TrainNetworks(network, SuperSet);
            //Lets create an evaluation.
            Console.WriteLine("Paused , Press a key to continue, Last Error was:" + error);
        }

        #region temporal etc.
        private static void SuperTemporalVersusBasic()
        {
           

           


            const string fileName = "c:\\EURUSD_Hourly_Bid_1999.03.04_2011.03.14.csv";
            List<double> Opens = NetworkUtility.QuickParseCSV(fileName, "Open", 1200);
            List<double> High = NetworkUtility.QuickParseCSV(fileName, "High", 1200);
            List<double> Low = NetworkUtility.QuickParseCSV(fileName, "Low", 1200);
            List<double> Close = NetworkUtility.QuickParseCSV(fileName, "Close", 1200);
            List<double> Volume = NetworkUtility.QuickParseCSV(fileName, 5, 1200);

            Encog.Util.Arrayutil.NormalizeArray ArrayNormalizer = new Encog.Util.Arrayutil.NormalizeArray();


            //This below doesn't work.
            ////lets normalize..
            //var trains = new BasicMLDataSet(Generate(1, 1200, ArrayNormalizer.Process(Opens.ToArray())),
            //                                Generate(1, 1200,ArrayNormalizer.Process(Close.ToArray())));




            TemporalMLDataSet superTemportal = new TemporalMLDataSet(100, 1);

            superTemportal = NetworkUtility.GenerateTrainingWithPercentChangeOnSerie(100, 1, Opens.ToArray(),
                                                                                                  Close.ToArray(), High.ToArray(), Low.ToArray(), Volume.ToArray());

            IMLDataPair aPairInput = ProcessPairs(NetworkUtility.CalculatePercents(Opens.ToArray()), NetworkUtility.CalculatePercents(Close.ToArray()), 100, 1);
            IMLDataPair aPairInput3 = ProcessPairs(NetworkUtility.CalculatePercents(Close.ToArray()), NetworkUtility.CalculatePercents(Close.ToArray()), 100, 1);
            IMLDataPair aPairInput2 = ProcessPairs(NetworkUtility.CalculatePercents(High.ToArray()), NetworkUtility.CalculatePercents(Close.ToArray()), 100, 1);
            IMLDataPair aPairInput4 = ProcessPairs(NetworkUtility.CalculatePercents(Volume.ToArray()), NetworkUtility.CalculatePercents(Close.ToArray()), 100, 1);

            List<IMLDataPair> listData = new List<IMLDataPair>();
            listData.Add(aPairInput);
            listData.Add(aPairInput2);
            listData.Add(aPairInput3);
            listData.Add(aPairInput4);



            var minitrainning = new BasicMLDataSet(listData);
            //make a network.
            var network = (BasicNetwork)CreateElmanNetwork(minitrainning.InputSize,minitrainning.IdealSize);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //Train the elmhan network with a basic dataset.
            double errorfirst = TrainNetworks(network, minitrainning);
            sw.Stop();
            double firsttime = sw.ElapsedMilliseconds;

            Console.WriteLine("First network trained in :" + sw.ElapsedMilliseconds);
            //train elhman with a temporal dataset.
            sw.Reset();
            sw.Start();
            double errorSecond = TrainNetworks(network, superTemportal);
            sw.Stop();
            Console.WriteLine("First network trained in :" + sw.ElapsedMilliseconds);
            double secondtime = sw.ElapsedMilliseconds;
            Console.WriteLine("First network trained in :" + firsttime + " Error for first network:" + errorfirst + " Second network error:" + errorSecond + "  took:" + secondtime);

            //Lets create an evaluation.
            Console.WriteLine("Paused , Press a key to continue to evaluation");
            Console.ReadKey();


            //lets evalute networks.
            CreateEvaluationSet(fileName);
        }

#endregion

        #region evaluation
        private static void CreateEvaluationSet(string @fileName)
        {
            List<double> Opens = NetworkUtility.QuickParseCSV(fileName, "Open", 1200,1200);
            List<double> High = NetworkUtility.QuickParseCSV(fileName, "High", 1200,1200);
            List<double> Low = NetworkUtility.QuickParseCSV(fileName, "Low", 1200,1200);
            List<double> Close = NetworkUtility.QuickParseCSV(fileName, "Close", 1200,1200);
            List<double> Volume = NetworkUtility.QuickParseCSV(fileName, 5, 1200,1200);
            TemporalMLDataSet superTemportal = new TemporalMLDataSet(100, 1);

            superTemportal = SuperUtility.NetworkUtility.GenerateTrainingWithPercentChangeOnSerie(100, 1, Opens.ToArray(),
                                                                                                   Close.ToArray(), High.ToArray(), Low.ToArray(), Volume.ToArray());


            IMLDataPair aPairInput = ProcessPairs(NetworkUtility.CalculatePercents(Opens.ToArray()), NetworkUtility.CalculatePercents(Close.ToArray()), 100, 1);
            IMLDataPair aPairInput3 = ProcessPairs(NetworkUtility.CalculatePercents(Close.ToArray()), NetworkUtility.CalculatePercents(Close.ToArray()), 100, 1);
            IMLDataPair aPairInput2 = ProcessPairs(NetworkUtility.CalculatePercents(High.ToArray()), NetworkUtility.CalculatePercents(Close.ToArray()), 100, 1);
            IMLDataPair aPairInput4 = ProcessPairs(NetworkUtility.CalculatePercents(Volume.ToArray()), NetworkUtility.CalculatePercents(Close.ToArray()), 100, 1);

            List<IMLDataPair> listData = new List<IMLDataPair>();
            listData.Add(aPairInput);
            listData.Add(aPairInput2);
            listData.Add(aPairInput3);
            listData.Add(aPairInput4);

            var minitrainning = new BasicMLDataSet(listData);

            var network = (BasicNetwork)CreateElmanNetwork(100,1);
            double normalCorrectRate = EvaluateNetworks(network, minitrainning);
            double temporalErrorRate = EvaluateNetworks(network, superTemportal);

            Console.WriteLine("Percent Correct with normal Data Set:" + normalCorrectRate + " Percent Correnct with temporal Dataset:" +
                      temporalErrorRate);
            Console.WriteLine("Paused , Press a key to continue to evaluation");
            Console.ReadKey();
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

      

        #region evaluate and train networks.
        public static double EvaluateNetworks(BasicNetwork network, TemporalMLDataSet set)
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
            for (int i = 0; i < 500; i++)
            {
                trainMain.Iteration();
                Console.WriteLine("Iteration #:" + trainMain.IterationNumber + " Error:"+trainMain.Error);
            }
            sw.Stop();

            return trainMain.Error;
        }

        #endregion


        public static IMLDataSet ProcessLive(double[] data, int _inputWindow, int _predictWindow)
        {
            IMLDataSet result = new BasicMLDataSet();
            for (int i = 0; i < data.Length; i++)
            {
                IMLData inputData = new BasicMLData(_inputWindow);
                IMLData idealData = new BasicMLData(_predictWindow);
                int index = i;
                // handle input window
                for (int j = 0; j < _inputWindow; j++)
                {
                    inputData[j] = data[index++];
                }

                // handle predict window
                for (int j = 0; j < _predictWindow; j++)
                {
                    idealData[j] = data[index++];
                }
                IMLDataPair pair = new BasicMLDataPair(inputData, idealData);
                result.Add(pair);
            }
            return result;
        }


        public static double[][] ProcessInputs(params double[][] data)
        {
            double[][] inputsremade = new double[data.Length][];
            //foreach (double[] t in data)
            //{
            //        int index2 = 0;
            //    foreach (double [] d in inputsremade)
            //    {
            //        EngineArray.ArrayCopy(t, inputsremade[index2++]);
            //    }
                   
               
            //}
            return data;

        }

        public static IMLDataSet ProcessLiveFullz(int _inputWindow, int _predictWindow, params double [][]data)
        {
            IMLDataSet result = new BasicMLDataSet();
            for (int i = 0; i < data.Length; i++)
            {
                IMLData inputData = new BasicMLData(data.Length);
                IMLData idealData = new BasicMLData(data.Length);
                int index2 = 0;
                double[][] inputsremade = new double[data.Length][];


                foreach (double[] doublearrs in data)
                {
                    EngineArray.ArrayCopy(doublearrs, inputsremade[index2++]);

                }
                int index = i;
                // handle input window
                  foreach (double[] doublearr in data)
                {
                    //We can now check the first array.
                    int indexArray = 0;
                    int InsideArray = 0;
                    foreach (double d in doublearr)
                    {
                        inputData.Data[InsideArray++] = d;
                    }

                }

                //for (int j = 0; j < _inputWindow; j++)
                //{
                //    inputData[j] = data[i][index+];
                //}

                //// handle predict window
                //for (int j = 0; j < _predictWindow; j++)
                //{
                //    idealData[j] = data[i][index++];
                //}
                IMLDataPair pair = new BasicMLDataPair(inputData, idealData);
                result.Add(pair);
            }
            return result;
        }


        public static IMLDataPair ProcessPairs(double[] data,double []ideal ,  int _inputWindow, int _predictWindow)
        {
            IMLDataSet result = new BasicMLDataSet();
            for (int i = 0; i < data.Length; i++)
            {
                IMLData inputData = new BasicMLData(_inputWindow);
                IMLData idealData = new BasicMLData(_predictWindow);
                int index = i;
                // handle input window
                for (int j = 0; j < _inputWindow; j++)
                {
                    inputData[j] = data[index++];
                }
                index = 0;
                // handle predict window
                for (int j = 0; j < _predictWindow; j++)
                {
                    idealData[j] = ideal[index++];
                }
                IMLDataPair pair = new BasicMLDataPair(inputData, idealData);
                return pair;
            }
            return null;
        }





        /// <summary>
        /// Creates the elman network.
        /// </summary>
        /// <param name="inputsize">The input neuron size.</param>
        /// <param name="outputsize">The outputsize.</param>
        /// <returns></returns>
        private static IMLMethod CreateElmanNetwork(int inputsize, int outputsize)
        {
            // construct an Elman type network
            ElmanPattern pattern = new ElmanPattern();
            pattern.ActivationFunction = new ActivationTANH();
            pattern.InputNeurons = inputsize;
            pattern.AddHiddenLayer(20);
            pattern.OutputNeurons = outputsize;
            return pattern.Generate();
        }



        public static double[] MakeInputs(int number)
        {
            Random rdn = new Random();

            double [] x = new double[number];
            for (int i = 0; i < number; i++)
            {
                x[i] = rdn.NextDouble();
            }
            return x;
        }
        public static double[][] CreateIdealOrInput(int nbofSeCondDimendsion,double []ideal, params object[] inputs)
        {
            double[][] result = EngineArray.AllocateDouble2D(inputs.Length, nbofSeCondDimendsion);
            int i = 0, k = 0;
            foreach (double[] doubles in inputs)
            {
                foreach (double d in doubles)
                {
                    result[i][k] = d;
                    k++;
                }
                if (i < inputs.Length - 1)
                {
                    i++;
                    k = 0;
                }
            }
            return result;
        }

        private static double[][] GenerateNews(int columns, params double[][] inputs)
        {
            double[][] result = EngineArray.AllocateDouble2D(inputs.Length, columns);

            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result[i][j] = inputs[i][j];
                }
            }
            return result;
        }


        private static double[][] Generate(int rows, int columns)
        {
            double[][] result = EngineArray.AllocateDouble2D(rows, columns);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result[i][j] = ThreadSafeRandom.NextDouble();
                }
            }

            return result;
        }

        private static double[][] Generate(int rows, int columns, params double[][] inputs)
        {
            double[][] result = EngineArray.AllocateDouble2D(rows, columns);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result[i][j] = inputs[i][j];
                }
            }
            return result;
        }

        /// <summary>
        /// Processes the specified double serie into an IMLDataset.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="_inputWindow">The _input window.</param>
        /// <param name="_predictWindow">The _predict window.</param>
        /// <returns></returns>
        public static IMLDataPair ProcessDoubleSerieIntoIMLDataset(double[] data, int _inputWindow, int _predictWindow)
        {

            int totalWindowSize = _inputWindow + _predictWindow;
            int stopPoint = data.Length - totalWindowSize;
            for (int i = 0; i < stopPoint; i++)
            {
                IMLData inputData = new BasicMLData(_inputWindow);
                IMLData idealData = new BasicMLData(_predictWindow);
                int index = i;
                // handle input window
                for (int j = 0; j < _inputWindow; j++)
                {
                    inputData[j] = data[index++];
                }
                // handle predict window
                for (int j = 0; j < _predictWindow; j++)
                {
                    idealData[j] = data[index++];
                }
                var pair = new BasicMLDataPair(inputData, idealData);
                return pair;
            }
            return null;
        }





    }
}
