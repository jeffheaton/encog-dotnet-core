//
// Encog(tm) Console Examples v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using Encog.ML.Data;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Strategy;
using Encog.Util.Logging;
using Encog.Examples.Util;
using Encog.Engine.Network.Activation;
using Encog.Neural.Pattern;
using Encog.ML;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;

namespace Encog.Examples.ElmanNetwork
{
    public class ElmanExample: IExample
    {
        private IExampleInterface app;

        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(ElmanExample),
                    "xor-elman",
                    "Elman Temporal XOR",
                    "Uses a temporal sequence, made up of the XOR truth table, as the basis for prediction.  Compares Elman to traditional feedforward.");
                return info;
            }
        }

        private IMLMethod CreateElmanNetwork()
        {
            // construct an Elman type network
            ElmanPattern pattern = new ElmanPattern();
            pattern.ActivationFunction = new ActivationSigmoid();
            pattern.InputNeurons = 1;
            pattern.AddHiddenLayer(6);
            pattern.OutputNeurons = 1;
            return pattern.Generate();
        }

        private IMLMethod CreateFeedforwardNetwork()
        {
            // construct a feedforward type network
            FeedForwardPattern pattern = new FeedForwardPattern();
            pattern.ActivationFunction = new ActivationSigmoid();
            pattern.InputNeurons = 1;
            pattern.AddHiddenLayer(6);
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

            StopTrainingStrategy stop = new StopTrainingStrategy();
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

        public void Execute(IExampleInterface app)
        {
            this.app = app;

            TemporalXOR temp = new TemporalXOR();
            IMLDataSet trainingSet = temp.Generate(100);

            BasicNetwork elmanNetwork = (BasicNetwork)CreateElmanNetwork();
            BasicNetwork feedforwardNetwork = (BasicNetwork)CreateFeedforwardNetwork();

            double elmanError = TrainNetwork("Elman", elmanNetwork, trainingSet);
            double feedforwardError = TrainNetwork("Feedforward", feedforwardNetwork, trainingSet);

            app.WriteLine("Best error rate with Elman Network: " + elmanError);
            app.WriteLine("Best error rate with Feedforward Network: " + feedforwardError);
            app.WriteLine("(Elman should outperform feed forward)");
            app.WriteLine("If your results are not as good, try rerunning, or perhaps training longer.");
        }

    }
}
