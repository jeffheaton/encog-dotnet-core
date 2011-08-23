//using System;
//using System.IO;
//using Encog.ML.Data;
//using Encog.ML.Train;
//using Encog.ML.Train.Strategy;
//using Encog.Neural.Networks;
//using Encog.Neural.Networks.Training;
//using Encog.Neural.Networks.Training.Anneal;
//using Encog.Neural.Networks.Training.Propagation.Back;
//using Encog.Persist;
//using Encog.Util.File;
//using Encog.Util.Simple;

//namespace Encog.Examples.RangeAndPredictions
//{

//    public class MarketTrain
//    {
//        public static void Train(FileInfo dataDir)
//        {
//            FileInfo networkFile = FileUtil.CombinePath(dataDir, RangeCalculators.RangeConfig.NETWORK_FILE);
//            FileInfo trainingFile = FileUtil.CombinePath(dataDir, RangeCalculators.RangeConfig.TRAINING_FILE);

//            // network file
//            if (!networkFile.Exists)
//            {
//                Console.WriteLine(@"Can't read file: " + networkFile);
//                return;
//            }

//       //     var network = (BasicNetwork) Utilities.LoadNetwork(Environment.CurrentDirectory, RangeConfig.NETWORK_FILE);

//            BasicNetwork network = (BasicNetwork)MarketBuildTraining.NetworkBuilders.CreateFeedforwardNetwork(RangeCalculators.RangeConfig.INPUT_WINDOW,
//                                                                        RangeCalculators.RangeConfig.PREDICT_WINDOW,
//                                                                        RangeCalculators.RangeConfig.HIDDEN1_COUNT,
//                                                                        RangeCalculators.RangeConfig.HIDDEN2_COUNT);
//            // training file
//            if (!trainingFile.Exists)
//            {
//                Console.WriteLine(@"Can't read file: " + trainingFile);
//                return;
//            }

//            var trainingSet = EncogUtility.LoadEGB2Memory(trainingFile);

//            // train the neural network
//            //  EncogUtility.TrainConsole(network, trainingSet, RangeConfig.TRAINING_MINUTES);

//            TrainNetwork("Feedforward", network, trainingSet);

//            //Console.WriteLine(@"Final Error: " + network.CalculateError(trainingSet));
//            Console.WriteLine(@"Training complete, saving network.");
//            EncogDirectoryPersistence.SaveObject(networkFile, network);
//            Console.WriteLine(@"Network saved.");
//            EncogFramework.Instance.Shutdown();
//        }


//        private static double TrainNetwork(String what, BasicNetwork network, IMLDataSet set)
//        {
//            // train the neural network
//            ICalculateScore score = new TrainingSetScore(set);
//            IMLTrain trainAlt = new NeuralSimulatedAnnealing(
//                network, score, 10, 2, 100);


//            IMLTrain trainMain = new Backpropagation(network, set, 0.00001, 0.0);

//            var stop = new StopTrainingStrategy();
//            trainMain.AddStrategy(new Greedy());
//            //trainMain.AddStrategy(new HybridStrategy(trainAlt));
//            trainMain.AddStrategy(stop);

//            int epoch = 0;
//            int geneticIterations = 0;

//            while (!stop.ShouldStop())
//            {
//                if (trainAlt.IterationNumber > geneticIterations)
//                {
//                    Console.WriteLine(@"Current Genetic Iteration:" + trainAlt.IterationNumber);
//                    geneticIterations = trainAlt.IterationNumber;

//                }
//                trainMain.Iteration();
//                Console.WriteLine(@"Training " + what + @", Epoch #" + epoch + @" Error:" + trainMain.Error + @" Genetic #" + trainAlt.IterationNumber + @" Genetic Error:" + trainAlt.Error);
//                epoch++;
//            }
//            //EncogUtility.TrainDialog(trainMain, network, trainingSet);

//            if (what.Equals(@"Feedforward"))
//            {
//                Encog.Util.NetworkUtil.NetworkUtility.SaveNetwork(Environment.CurrentDirectory, RangeCalculators.RangeConfig.NETWORK_FILE,
//                                                                  network);
//                Console.WriteLine(@"Saved the Feedforward network");
//            }


//            return trainMain.Error;
//        }
//    }
//}