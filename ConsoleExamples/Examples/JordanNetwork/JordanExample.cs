//
// Encog(tm) Core v3.1 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2012 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using ConsoleExamples.Examples;
using Encog.Engine.Network.Activation;
using Encog.Examples.Util;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Pattern;

namespace Encog.Examples.JordanNetwork
{
    /// <summary>
    /// Implement an Jordan style neural network with Encog. This network attempts to
    /// predict the next value in an XOR sequence, taken one at a time. A regular
    /// feedforward network would fail using a single input neuron for this task. The
    /// internal state stored by an Jordan neural network allows better performance.
    /// 
    /// This example does not perform very well and is provided mainly as a contrast to
    /// the ExlmanXOR.  There is only one context neuron, because there is only one output 
    /// neuron.  This network does not perform as well as the Elman for XOR.
    /// 
    /// Jordan is better suited to a larger array of output neurons.
    /// </summary>
    public class JordanExample : IExample
    {
        private IExampleInterface app;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (JordanExample),
                    "xor-jordan",
                    "Jordan Temporal XOR",
                    "Uses a temporal sequence, made up of the XOR truth table, as the basis for prediction.  Compares Jordan to traditional feedforward.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            this.app = app;

            var temp = new TemporalXOR();
            IMLDataSet trainingSet = temp.Generate(100);

            var jordanNetwork = (BasicNetwork) CreateJordanNetwork();
            var feedforwardNetwork = (BasicNetwork) CreateFeedforwardNetwork();

            double elmanError = TrainNetwork("Jordan", jordanNetwork, trainingSet);
            double feedforwardError = TrainNetwork("Feedforward", feedforwardNetwork, trainingSet);

            app.WriteLine("Best error rate with Jordan Network: " + elmanError);
            app.WriteLine("Best error rate with Feedforward Network: " + feedforwardError);
            app.WriteLine("Jordan will perform only marginally better than feedforward.\nThe more output neurons, the better performance a Jordan will give.");
        }

        #endregion

        private IMLMethod CreateJordanNetwork()
        {
            // construct an Jordan type network
            var pattern = new JordanPattern
                {
                    ActivationFunction = new ActivationSigmoid()
                };
            pattern.InputNeurons = 1;
            pattern.AddHiddenLayer(2);
            pattern.OutputNeurons = 1;
            return pattern.Generate();
        }

        private IMLMethod CreateFeedforwardNetwork()
        {
            // construct a feedforward type network
            var pattern = new FeedForwardPattern();
            pattern.ActivationFunction = new ActivationSigmoid();
            pattern.InputNeurons = 1;
            pattern.AddHiddenLayer(2);
            pattern.OutputNeurons = 1;
            return pattern.Generate();
        }

        private double TrainNetwork(String what, BasicNetwork network, IMLDataSet trainingSet)
        {
            // train the neural network
            ICalculateScore score = new TrainingSetScore(trainingSet);
            IMLTrain trainAlt = new NeuralSimulatedAnnealing(
                network, score, 10, 2, 100);


            IMLTrain trainMain = new Backpropagation(network, trainingSet, 0.00001, 0.0);

            var stop = new StopTrainingStrategy();
            trainMain.AddStrategy(new Greedy());
            trainMain.AddStrategy(new HybridStrategy(trainAlt));
            trainMain.AddStrategy(stop);

            int epoch = 0;
            while (!stop.ShouldStop())
            {
                trainMain.Iteration();
                app.WriteLine("Training " + what + ", Epoch #" + epoch + " Error:" + trainMain.Error);
                epoch++;
            }
            return trainMain.Error;
        }
    }
}
