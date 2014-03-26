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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Util.Simple;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks;
using Encog.MathUtil.Randomize;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.PSO;

namespace Encog.Examples.XOR
{
    public class XORPSO : IExample
    {
        /// <summary>
        /// Input for the XOR function.
        /// </summary>
        public static double[][] XORInput = {
            new[] {0.0, 0.0},
            new[] {1.0, 0.0},
            new[] {0.0, 1.0},
            new[] {1.0, 1.0}
        };

        /// <summary>
        /// Ideal output for the XOR function.
        /// </summary>
        public static double[][] XORIdeal = {
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
                    typeof(XORPSO),
                    "xor-pso",
                    "Simple XOR with PSO (particle stream) training.",
                    "This example shows how to train an XOR and PSO.");
                return info;
            }
        }

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="app">Holds arguments and other info.</param>
        public void Execute(IExampleInterface app)
        {
            IMLDataSet trainingSet = new BasicMLDataSet(XORInput, XORIdeal);
            BasicNetwork network = EncogUtility.SimpleFeedForward(2, 2, 0, 1, false);
            ICalculateScore score = new TrainingSetScore(trainingSet);
            IRandomizer randomizer = new NguyenWidrowRandomizer();

            IMLTrain train = new NeuralPSO(network, randomizer, score, 20);

            EncogUtility.TrainToError(train, 0.01);

            network = (BasicNetwork)train.Method;

            // test the neural network
            Console.WriteLine("Neural Network Results:");
            EncogUtility.Evaluate(network, trainingSet);

            EncogFramework.Instance.Shutdown();
        }
    }
}
