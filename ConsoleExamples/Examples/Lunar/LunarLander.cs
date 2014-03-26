//
// Encog(tm) Core v3.2 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
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
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Pattern;
using Encog.MathUtil.Randomize;
using Encog.ML.Genetic;
using Encog.ML;

namespace Encog.Examples.Lunar
{
    /// <summary>
    /// A lunar lander game where the neural network learns to land a space craft.  
    /// The neural network learns the proper amount of thrust to land softly 
    /// and conserve fuel.
    /// 
    /// This example is unique because it uses supervised training, yet does not 
    /// have expected values.  For this it can use genetic algorithms or 
    /// simulated annealing.
    /// </summary>
    public class LunarLander : IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(LunarLander),
                    "lander",
                    "Train a neural network to land a space ship.",
                    "This example uses a genetic algorithm to land a space craft on the moon.  No training data is given, but it uses supervised training to land.");
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
            BasicNetwork network = CreateNetwork();

            IMLTrain train;

            if (app.Args.Length > 0 && String.Compare(app.Args[0], "anneal", true) == 0)
            {
                train = new NeuralSimulatedAnnealing(
                    network, new PilotScore(), 10, 2, 100);
            }
            else
            {
                train = new MLMethodGeneticAlgorithm( ()=>{
					BasicNetwork result = CreateNetwork();
					((IMLResettable)result).Reset();
					return result;
				},new PilotScore(),500);
            }

            int epoch = 1;

            for (int i = 0; i < 50; i++)
            {
                train.Iteration();
                Console.WriteLine(@"Epoch #" + epoch + @" Score:" + train.Error);
                epoch++;
            }

            Console.WriteLine(@"\nHow the winning network landed:");
            network = (BasicNetwork) train.Method;
            var pilot = new NeuralPilot(network, true);
            Console.WriteLine(pilot.ScorePilot());
            EncogFramework.Instance.Shutdown();
        }

        #endregion

        public static BasicNetwork CreateNetwork()
        {
            var pattern = new FeedForwardPattern {InputNeurons = 3};
            pattern.AddHiddenLayer(50);
            pattern.OutputNeurons = 1;
            pattern.ActivationFunction = new ActivationTANH();
            var network = (BasicNetwork) pattern.Generate();
            network.Reset();
            return network;
        }
    }
}
