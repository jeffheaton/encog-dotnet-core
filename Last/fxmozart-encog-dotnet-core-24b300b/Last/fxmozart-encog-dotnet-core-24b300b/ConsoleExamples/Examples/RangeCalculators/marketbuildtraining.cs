using System;
using System.IO;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Data.Market;
using Encog.ML.Data.Market.Loader;
using Encog.ML.Data.Temporal;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Pattern;
using Encog.Persist;
using Encog.Util.File;
using Encog.Util.Simple;

namespace Encog.Examples.RangeCalculators
{
    public class MarketBuildTraining
    {
        public static void Generate(string fileName)
        {


            FileInfo dataDir = new FileInfo(@Environment.CurrentDirectory);
            IMarketLoader loader = new RangeMaker();
            loader.GetFile(@fileName);

            var market = new MarketMLDataSet(loader, RangeConfig.INPUT_WINDOW, RangeConfig.PREDICT_WINDOW);
            
            
            //var descClose = new MarketDataDescription(RangeConfig.TICKER, MarketDataType.Close,TemporalDataDescription.Type.PercentChange, true, false);
            //market.AddDescription(descClose);
            //var descOpen = new MarketDataDescription(RangeConfig.TICKER, MarketDataType.Open, TemporalDataDescription.Type.PercentChange, true, false);
            //market.AddDescription(descOpen);
            //var descHigh = new MarketDataDescription(RangeConfig.TICKER, MarketDataType.High, TemporalDataDescription.Type.PercentChange, true, false);
            //market.AddDescription(descHigh);
            //var descLow = new MarketDataDescription(RangeConfig.TICKER, MarketDataType.Low, TemporalDataDescription.Type.PercentChange, true, false);
            //market.AddDescription(descLow);
            //var ranges = new MarketDataDescription(RangeConfig.TICKER, MarketDataType.RangeHighLow,
            //                                       TemporalDataDescription.Type.Raw, true, false);
           // market.AddDescription(ranges);
            var RangeopenClose = new MarketDataDescription(RangeConfig.TICKER, MarketDataType.RangeOpenClose,
                                                           TemporalDataDescription.Type.Raw, true, true);
            market.AddDescription(RangeopenClose);
            string currentDirectory = @"c:\EncogOutput";
            var end = DateTime.Now; // end today
            var begin = new DateTime(end.Ticks); // begin 30 days ago
            // Gather training data for the last 2 years, stopping 60 days short of today.
            // The 60 days will be used to evaluate prediction.
            begin = begin.AddDays(-800);
            end = begin.AddDays(800);

            Console.WriteLine(@"You are loading date from:" + begin.ToShortDateString() + @" To :" +
                              end.ToShortDateString());

            market.Load(begin, end);
            market.Generate();
            EncogUtility.SaveEGB(FileUtil.CombinePath(dataDir, RangeConfig.TRAINING_FILE), market);

            // create a network



            Console.WriteLine(@"Built training file , and saved it to:" + dataDir.DirectoryName);

            //BasicNetwork network =
            //    (BasicNetwork) MarketBuildTraining.NetworkBuilders.CreateFeedforwardNetwork(RangeConfig.INPUT_WINDOW,
            //                                                                                RangeConfig.PREDICT_WINDOW,
            //                                                                                RangeConfig.HIDDEN1_COUNT,
            //                                                                                RangeConfig.HIDDEN2_COUNT);
            // training file




            // train the neural network
            //  EncogUtility.TrainConsole(network, trainingSet, RangeConfig.TRAINING_MINUTES);
            NetworkBuilders.DualNetworksTrainer(market.InputNeuronCount, market.IdealSize,
                                                RangeConfig.HIDDEN1_COUNT, RangeConfig.HIDDEN2_COUNT, market,
                                                RangeConfig.NETWORK_FILE);


        }




        #region networks builders.
        public class NetworkBuilders
        {
            /// <summary>
            /// Creates the feedforward network.
            /// </summary>
            /// <param name="inputneurons">The inputneurons.</param>
            /// <param name="outputsneurons">The outputsneurons.</param>
            /// <param name="hiddenNeurons">The hidden neurons.</param>
            /// <param name="hidden2">The hidden neurons 2.</param>
            /// <returns></returns>
            public static IMLMethod CreateFeedforwardNetwork(int inputneurons, int outputsneurons, int hiddenNeurons, int hidden2)
            {
                // construct a feedforward type network
                FeedForwardPattern pattern = new FeedForwardPattern();
                pattern.ActivationFunction = new ActivationTANH();
                pattern.InputNeurons = inputneurons;
                pattern.AddHiddenLayer(hiddenNeurons);
                pattern.AddHiddenLayer(hidden2);
                pattern.OutputNeurons = outputsneurons;
                return pattern.Generate();
            }



            /// <summary>
            /// Trains 2 networks using a combo of Genetic algos and resilients / back propagations.
            /// Strategies:
            /// Stop strategy , Greed Strategy , and Genetic algo strategy.
            /// </summary>
            /// <param name="inputneurons">The inputneurons.</param>
            /// <param name="outputneurons">The outputneurons.</param>
            /// <param name="hiddenneurons">The hiddenneurons.</param>
            /// <param name="trainingSet">The training set.</param>
            /// <param name="m_logger">The m_logger.</param>
            /// <param name="netfile">The netfile.</param>
            public static void DualNetworksTrainer(int inputneurons, int outputneurons, int hiddenneurons, int hidden2, MarketMLDataSet trainingSet, string netfile)
            {
                // IMLDataSet trainingSet = LoadTraining(Config.DIRECTORY, Config.MARKET_TrainingFile);

                Console.WriteLine("Inside Dual Network Trainer");
                BasicNetwork feedforwardNetwork = (BasicNetwork)CreateFeedforwardNetwork(inputneurons, outputneurons, hiddenneurons, hidden2);
                //  BasicNetwork Elhman = (BasicNetwork)CreateElmanNetwork(inputneurons, outputneurons, hiddenneurons);
                //  double elmanError = TrainNetwork("Elhman", Elhman, trainingSet, m_logger);
                double feedforwardError = TrainNetwork("Feedforward", feedforwardNetwork, trainingSet,netfile);
                //m_logger.Info("Best error rate with Elmhan Network: " + elmanError);
                Console.WriteLine("Best error rate with Feedforward Network: " + feedforwardError);
                Environment.Exit(0);
            }



     
            private static double TrainNetwork(String what, BasicNetwork network, MarketMLDataSet set, string netfile)
            {
                // train the neural network
                ICalculateScore score = new TrainingSetScore(set);
                IMLTrain trainAlt = new NeuralSimulatedAnnealing(
                    network, score, 10, 2, 1);


                IMLTrain trainMain = new Backpropagation(network, set, 0.00001, 0.0);

                var stop = new StopTrainingStrategy();
                trainMain.AddStrategy(new Greedy());
                trainMain.AddStrategy(new HybridStrategy(trainAlt));
                trainMain.AddStrategy(stop);

                int epoch = 0;
                int geneticIterations = 0;

                while (!stop.ShouldStop())
                {
                    
                    trainMain.Iteration();
                    Console.WriteLine("Training " + what + ", Epoch #" + epoch + " Error:" + trainMain.Error + " Genetic #" + trainAlt.IterationNumber + " Genetic Error:" + trainAlt.Error);
                    epoch++;
                }
                //EncogUtility.TrainDialog(trainMain, network, trainingSet);

                if (what.Equals("Feedforward"))
                {
                    Encog.Util.NetworkUtil.NetworkUtility.SaveNetwork(RangeConfig.DIRECTORY, RangeConfig.NETWORK_FILE,
                                                                      network);
                    Console.WriteLine("Saved the Feedforward network");
                }
                if (what.Equals("Elmahn"))
                {
                    Encog.Util.NetworkUtil.NetworkUtility.SaveNetwork(RangeConfig.DIRECTORY, netfile,
                                                                      network);
                    Console.WriteLine("Saved the Elhman network");
                }

                return trainMain.Error;
            }
        }
        #endregion
    }
}