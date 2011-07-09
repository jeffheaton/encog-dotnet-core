using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using Encog.Persist;
using Encog.Util.Simple;
using Encog.ML.Data.Buffer;
using Encog.Util.CSV;

namespace Encog.Examples.Forest
{
    public class TrainNetwork
    {
        private ForestConfig config;

        public TrainNetwork(ForestConfig config)
        {
            this.config = config;
        }

        public void train(bool useGUI)
        {
            // load, or create the neural network
            BasicNetwork network = null;

            if (!config.TrainedNetworkFile.Exists)
            {
                throw new EncogError("Can't find neural network file, please generate data");
            }

            network = (BasicNetwork)EncogDirectoryPersistence.LoadObject(config.TrainedNetworkFile);

            // convert training data
            Console.WriteLine("Converting training file to binary");




            EncogUtility.ConvertCSV2Binary(
                config.NormalizedDataFile.ToString(),
                CSVFormat.English,
                config.BinaryFile.ToString(),
                network.InputCount,
                network.OutputCount,
                false, false);

            BufferedMLDataSet trainingSet = new BufferedMLDataSet(
                    config.BinaryFile.ToString());


            if (useGUI)
            {
                EncogUtility.TrainDialog(network, trainingSet);
            }
            else
            {
                EncogUtility.TrainConsole(network, trainingSet,
                        config.TrainingMinutes);
            }

            Console.WriteLine("Training complete, saving network...");
            EncogDirectoryPersistence.SaveObject(config.TrainedNetworkFile, network);
        }

    }
}
