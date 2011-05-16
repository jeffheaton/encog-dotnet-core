using System;
using ConsoleExamples.Examples;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Flat;
using Encog.Neural.Flat.Train.Prop;

namespace Encog.Examples.XOR
{
    public class XORFlat : IExample
    {
        /// <summary>
        /// Input for the XOR function.
        /// </summary>
        public static double[][] XORInput = {
                                                new double[2] {0.0, 0.0},
                                                new double[2] {1.0, 0.0},
                                                new double[2] {0.0, 1.0},
                                                new double[2] {1.0, 1.0}
                                            };

        /// <summary>
        /// Ideal output for the XOR function.
        /// </summary>
        public static double[][] XORIdeal = {
                                                new double[1] {0.0},
                                                new double[1] {1.0},
                                                new double[1] {1.0},
                                                new double[1] {0.0}
                                            };

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (XORFlat),
                    "xor-flat",
                    "XOR with flat network.",
                    "This example performs an XOR using only flat networks.");
                return info;
            }
        }

        #region IExample Members

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="app">Holds arguments and other info.</param>
        public void Execute(IExampleInterface app)
        {
            var network = new FlatNetwork(2, 4, 0, 1, false);
            network.Randomize();

            MLDataSet trainingSet = new BasicMLDataSet(XORInput, XORIdeal);


            var train = new TrainFlatNetworkResilient(network, trainingSet);

            int epoch = 1;

            do
            {
                train.Iteration();
                Console.WriteLine(@"Epoch #" + epoch + @" Error:" + train.Error);
                epoch++;
            } while (train.Error > 0.01);

            var output = new double[1];
            // test the neural network
            Console.WriteLine(@"Neural Network Results:");
            foreach (MLDataPair pair in trainingSet)
            {
                double[] input = pair.Input.Data;
                network.Compute(input, output);
                Console.WriteLine(input[0] + @"," + input[1] + @":" + output[0]);
            }
        }

        #endregion
    }
}