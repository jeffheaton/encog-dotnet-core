using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks;
using Encog.Neural.Activation;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Logic;
using Encog.Neural.Data.Buffer;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Multi;
using Encog.Persist;
using System.IO;

namespace Sandbox
{
    public class TrainNetwork
    {
        public static BasicNetwork GenerateNetwork(INeuralDataSet trainingSet)
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, trainingSet.InputSize));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, Constant.HIDDEN_COUNT));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, trainingSet.IdealSize));
            network.Logic = new FeedforwardLogic();
            network.Structure.FinalizeStructure();
            network.Reset();
            return network;
        }

        public void Train()
        {
            BufferedNeuralDataSet trainingSet = new BufferedNeuralDataSet(Constant.TRAINING_FILE);
            BasicNetwork network = GenerateNetwork(trainingSet);

            //final Train train = new ResilientPropagation(network, trainingSet);
            ITrain train = new MultiPropagation(network, trainingSet);

            int epoch = 1;
            long remaining;

            Console.WriteLine("Beginning training...");
            long start = (long)DateTime.Now.Ticks;
            do
            {
                train.Iteration();

                long current = (long)DateTime.Now.Ticks;
                long elapsed = (current - start);// seconds
                elapsed = elapsed / (long)Encog.Util.Banchmark.Evaluate.TICKS;
                elapsed = elapsed / (long)Encog.Util.Banchmark.Evaluate.MILIS;
                elapsed /= 60;// minutes
                remaining = Constant.TRAINING_MINUTES - elapsed;

                Console.WriteLine("Epoch #" + epoch + " Error:" + train.Error + " remaining minutes = " + remaining);
                epoch++;
            } while (remaining > 0);

            EncogPersistedCollection encog = new EncogPersistedCollection(Constant.TRAINED_NETWORK_FILE, FileMode.OpenOrCreate);
            encog.Add(Constant.TRAINED_NETWORK_NAME, network);
        }
    }
}
