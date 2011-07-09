using System;
using Encog.ML.Data.Buffer;
using Encog.Neural.Networks;
using Encog.Persist;
using Encog.Util.CSV;
using Encog.Util.Simple;

namespace Encog.Examples.Forest
{
    public class TrainNetwork
    {
        private readonly ForestConfig _config;

        public TrainNetwork(ForestConfig config)
        {
            _config = config;
        }

        public void Train(bool useGui)
        {
            // load, or create the neural network
            BasicNetwork network;

            if (!_config.TrainedNetworkFile.Exists)
            {
                throw new EncogError(@"Can't find neural network file, please generate data");
            }

            network = (BasicNetwork) EncogDirectoryPersistence.LoadObject(_config.TrainedNetworkFile);

            // convert training data
            Console.WriteLine(@"Converting training file to binary");


            EncogUtility.ConvertCSV2Binary(
                _config.NormalizedDataFile.ToString(),
                CSVFormat.English,
                _config.BinaryFile.ToString(),
                network.InputCount,
                network.OutputCount,
                false, false);

            var trainingSet = new BufferedMLDataSet(
                _config.BinaryFile.ToString());


            if (useGui)
            {
                EncogUtility.TrainDialog(network, trainingSet);
            }
            else
            {
                EncogUtility.TrainConsole(network, trainingSet,
                                          _config.TrainingMinutes);
            }

            Console.WriteLine(@"Training complete, saving network...");
            EncogDirectoryPersistence.SaveObject(_config.TrainedNetworkFile, network);
        }
    }
}