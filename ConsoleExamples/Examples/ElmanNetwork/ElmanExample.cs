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
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Lma;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Pattern;
using SuperUtils = Encog.Util.NetworkUtil.NetworkUtility;
using SuperUtilsTrainer = Encog.Util.NetworkUtil.TrainerHelper;
namespace Encog.Examples.ElmanNetwork
{
    public class ElmanExample : IExample
    {
        private IExampleInterface app;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (ElmanExample),
                    "xor-elman",
                    "Elman Temporal XOR",
                    "Uses a temporal sequence, made up of the XOR truth table, as the basis for prediction.  Compares Elman to traditional feedforward....You can also use xor-elman 300 to make a xor data set of 300 size");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            this.app = app;
            var temp = new TemporalXOR();
            IMLDataSet trainingSet = temp.Generate(100);
            if (app.Args.Length >0)
            {
                trainingSet = temp.Generate(Convert.ToInt16(app.Args[0]));
              
            }
        

            var elmanNetwork = (BasicNetwork) CreateElmanNetwork(trainingSet.InputSize);
            var feedforwardNetwork = (BasicNetwork) CreateFeedforwardNetwork(trainingSet.InputSize);

            double elmanError = TrainNetwork("Elman", elmanNetwork, trainingSet, "Leven");
            double feedforwardError = TrainNetwork("Feedforward", feedforwardNetwork, trainingSet, "Leven");

            app.WriteLine("Best error rate with Elman Network: " + elmanError);
            app.WriteLine("Best error rate with Feedforward Network: " + feedforwardError);
            app.WriteLine("(Elman should outperform feed forward)");
            app.WriteLine("If your results are not as good, try rerunning, or perhaps training longer.");
        }

        #endregion



        private IMLMethod CreateElmanNetwork(int input)
        {
            // construct an Elman type network
            var pattern = new ElmanPattern
                {
                    ActivationFunction = new ActivationSigmoid(),
                    InputNeurons = input
                };
            pattern.AddHiddenLayer(5);
            pattern.OutputNeurons = 1;
            return pattern.Generate();
        }

        private static IMLMethod CreateFeedforwardNetwork(int input)
        {
            // construct a feedforward type network
            var pattern = new FeedForwardPattern();
            pattern.ActivationFunction = new ActivationSigmoid();
            pattern.InputNeurons = input;
            pattern.AddHiddenLayer(5);
            pattern.OutputNeurons = 1;
            return pattern.Generate();
        }

        private double TrainNetwork(String what, BasicNetwork network, IMLDataSet trainingSet, string Method)
        {
            // train the neural network
            ICalculateScore score = new TrainingSetScore(trainingSet);
            IMLTrain trainAlt = new NeuralSimulatedAnnealing(network, score, 10, 2, 100);
            IMLTrain trainMain;
            if (Method.Equals("Leven"))
            {
                Console.WriteLine("Using LevenbergMarquardtTraining");
                trainMain = new LevenbergMarquardtTraining(network, trainingSet);
            }
            else
                 trainMain = new Backpropagation(network, trainingSet);

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
