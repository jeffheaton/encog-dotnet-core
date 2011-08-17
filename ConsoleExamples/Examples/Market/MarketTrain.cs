using System;
using System.IO;
using Encog.ML.Data;
using Encog.Neural.Networks;
using Encog.Persist;
using Encog.Util.File;
using Encog.Util.Simple;

namespace Encog.Examples.Market
{
    public class MarketTrain
    {
        public static void Train(FileInfo dataDir)
        {
            FileInfo networkFile = FileUtil.CombinePath(dataDir, Config.NETWORK_FILE);
            FileInfo trainingFile = FileUtil.CombinePath(dataDir, Config.TRAINING_FILE);

            // network file
            if (!networkFile.Exists)
            {
                Console.WriteLine(@"Can't read file: " + networkFile);
                return;
            }

            var network = (BasicNetwork) EncogDirectoryPersistence.LoadObject(networkFile);

            // training file
            if (!trainingFile.Exists)
            {
                Console.WriteLine(@"Can't read file: " + trainingFile);
                return;
            }

            IMLDataSet trainingSet = EncogUtility.LoadEGB2Memory(trainingFile);

            // train the neural network
            EncogUtility.TrainConsole(network, trainingSet, Config.TRAINING_MINUTES);

            Console.WriteLine(@"Final Error: " + network.CalculateError(trainingSet));
            Console.WriteLine(@"Training complete, saving network.");
            EncogDirectoryPersistence.SaveObject(networkFile, network);
            Console.WriteLine(@"Network saved.");

            EncogFramework.Instance.Shutdown();
        }
    }
}