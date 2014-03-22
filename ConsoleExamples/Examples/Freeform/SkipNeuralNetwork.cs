using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Freeform;
using Encog.Util.Simple;

namespace Encog.Examples.Freeform
{
    public class SkipNeuralNetwork: IExample
    {
        /// <summary>
        ///     Input for the XOR function.
        /// </summary>
        public static double[][] XORInput =
        {
            new[] {0.0, 0.0},
            new[] {1.0, 0.0},
            new[] {0.0, 1.0},
            new[] {1.0, 1.0}
        };

        /// <summary>
        ///     Ideal output for the XOR function.
        /// </summary>
        public static double[][] XORIdeal =
        {
            new[] {0.0},
            new[] {1.0},
            new[] {1.0},
            new[] {0.0}
        };

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(SkipNeuralNetwork),
                    "freeform-skip",
                    "Freeform Network: Skip network",
                    "Use a freeform neural network with connections that skip the hidden layer.");
                return info;
            }
        }

        public void Execute(IExampleInterface app)
        {
            // create a neural network, without using a factory
            FreeformNetwork network = new FreeformNetwork();
            var inputLayer = network.CreateInputLayer(2);
            var hiddenLayer1 = network.CreateLayer(3);
            var outputLayer = network.CreateOutputLayer(1);

            network.ConnectLayers(inputLayer, hiddenLayer1, new ActivationSigmoid(), 1.0, false);
            network.ConnectLayers(hiddenLayer1, outputLayer, new ActivationSigmoid(), 1.0, false);
            network.ConnectLayers(inputLayer, outputLayer, new ActivationSigmoid(), 0.0, false);

            network.Reset();

            // create training data
            IMLDataSet trainingSet = new BasicMLDataSet(XORInput, XORIdeal);
            //MLTrain train = new FreeformBackPropagation(network, trainingSet, 0.7, 0.3);
            //EncogUtility.trainToError(train, 0.01);
            EncogUtility.TrainToError(network, trainingSet, 0.01);
            EncogUtility.Evaluate(network, trainingSet);

            EncogFramework.Instance.Shutdown();
        }
    }
}
