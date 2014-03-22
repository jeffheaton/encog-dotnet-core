using System.Collections.Generic;
using ConsoleExamples.Examples;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Freeform;
using Encog.Neural.Freeform.Basic;
using Encog.Neural.Freeform.Training;
using Encog.Util.Simple;

namespace Encog.Examples.Freeform
{
    public class FreeformOnlineXOR : IExample
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
                    typeof(FreeformOnlineXOR),
                    "freeform-online-xor",
                    "Freeform Network: Online XOR",
                    "Use a freeform neural network to fit the XOR operator with online training.");
                return info;
            }
        }

        public void Execute(IExampleInterface app)
        {
            // create a neural network, without using a factory
            var network = new FreeformNetwork();
            IFreeformLayer inputLayer = network.CreateInputLayer(2);
            IFreeformLayer hiddenLayer1 = network.CreateLayer(3);
            IFreeformLayer outputLayer = network.CreateOutputLayer(1);

            network.ConnectLayers(inputLayer, hiddenLayer1, new ActivationSigmoid(), 1.0, false);
            network.ConnectLayers(hiddenLayer1, outputLayer, new ActivationSigmoid(), 1.0, false);

            network.Reset();

            // create training data
            var trainingSet = new BasicMLDataSet(XORInput, XORIdeal);
            var train = new FreeformBackPropagation(network, trainingSet, 0.7, 0.2);
            train.BatchSize = 1;
            
            EncogUtility.TrainToError(train, 0.01);
            EncogUtility.Evaluate(network, trainingSet);

            EncogFramework.Instance.Shutdown();
        }
    }
}