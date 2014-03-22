using ConsoleExamples.Examples;
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.Neural.Freeform;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Util.Simple;

namespace Encog.Examples.Freeform
{
    public class ConvertToFreeform: IExample
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
                    typeof(ConvertToFreeform),
                    "freeform-convert",
                    "Freeform Network: convert flat network to freeform",
                    "Create a flat network and convert to freeform");
                return info;
            }
        }

        public void Execute(IExampleInterface app)
        {
            // create a neural network, without using a factory
            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 2));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 3));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1));
            network.Structure.FinalizeStructure();
            network.Reset();

            // create training data
            var trainingSet = new BasicMLDataSet(XORInput, XORIdeal);
            EncogUtility.TrainToError(network, trainingSet, 0.01);
            EncogUtility.Evaluate(network, trainingSet);

            var ff = new FreeformNetwork(network);
            EncogUtility.Evaluate(ff, trainingSet);

            EncogFramework.Instance.Shutdown();
        }
    }
}
