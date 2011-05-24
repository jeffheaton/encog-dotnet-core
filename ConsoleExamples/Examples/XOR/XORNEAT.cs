using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.NEAT;
using Encog.Neural.Networks.Training;
using Encog.Engine.Network.Activation;
using Encog.Neural.NEAT.Training;
using Encog.Util.Simple;

namespace Encog.Examples.XOR
{
    public class XORNEAT : IExample
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
                    typeof(XORNEAT),
                    "xor-neat",
                    "Simple XOR with NEAT.",
                    "This example shows how to train an XOR using NEAT.");
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
            IMLDataSet trainingSet = new BasicMLDataSet(XORInput, XORIdeal);
            NEATPopulation pop = new NEATPopulation(2, 1, 1000);
            ICalculateScore score = new TrainingSetScore(trainingSet);
            // train the neural network
            ActivationStep step = new ActivationStep();
            step.Center = 0.5;
            pop.OutputActivationFunction = step;

            NEATTraining train = new NEATTraining(score, pop);

            EncogUtility.TrainToError(train, 0.01);

            NEATNetwork network = (NEATNetwork)train.Method;

            network.ClearContext();
            // test the neural network
            Console.WriteLine(@"Neural Network Results:");
            EncogUtility.Evaluate(network, trainingSet);
        }

        #endregion
    }
}
