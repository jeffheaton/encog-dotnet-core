using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Training;
using Encog.Neural.Activation;
using Encog.Neural.Networks.Training.NEAT;
using Encog.Neural.Data.Basic;
using Encog.Util.Simple;
using Encog.Neural.Networks;

namespace Encog.Examples.XOR.NEAT
{
    class XORNEAT: IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(XORNEAT),
                    "xor-neat",
                    "XOR Operator with NEAT neural network",
                    "Use NEAT to learn the XOR operator.");
                return info;
            }
        }
        /// <summary>
        /// Input for the XOR function.
        /// </summary>
        public static double[][] XOR_INPUT ={
            new double[2] { 0.0, 0.0 },
            new double[2] { 1.0, 0.0 },
			new double[2] { 0.0, 1.0 },
            new double[2] { 1.0, 1.0 } };

        /// <summary>
        /// Ideal output for the XOR function.
        /// </summary>
        public static double[][] XOR_IDEAL = {                                              
            new double[1] { 0.0 }, 
            new double[1] { 1.0 }, 
            new double[1] { 1.0 }, 
            new double[1] { 0.0 } };

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Not used.</param>
        public void Execute(IExampleInterface app)
        {
            INeuralDataSet trainingSet = new BasicNeuralDataSet(XOR_INPUT, XOR_IDEAL);

            ICalculateScore score = new TrainingSetScore(trainingSet);
            // train the neural network
            ActivationStep step = new ActivationStep();
            step.Center = 0.5;

            NEATTraining train = new NEATTraining(
                    score, 2, 1, 100);
            train.OutputActivationFunction = step;

            EncogUtility.TrainToError(train, trainingSet, 0.01);

            BasicNetwork network = train.Network;
            network.ClearContext();
            EncogUtility.Evaluate(network, trainingSet);
        }
    }
}
