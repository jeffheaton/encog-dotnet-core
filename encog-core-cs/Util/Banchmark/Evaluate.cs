//
// Encog(tm) Core v3.3 - .Net Version
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
using System.Diagnostics;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Util.Simple;

namespace Encog.Util.Banchmark
{
    /// <summary>
    /// Used to evaluate the training time for a network.
    /// </summary>
    public class Evaluate
    {
        /// <summary>
        /// Mili-seconds in a second.
        /// </summary>
        public const int Milis = 1000;

        /// <summary>
        /// Evaluate training.
        /// </summary>
        /// <param name="input">Input neurons.</param>
        /// <param name="hidden1">Hidden 1 neurons.</param>
        /// <param name="hidden2">Hidden 2 neurons.</param>
        /// <param name="output">Output neurons.</param>
        /// <returns>The result of the evaluation.</returns>
        public static int EvaluateTrain(int input, int hidden1, int hidden2,
                                        int output)
        {
            BasicNetwork network = EncogUtility.SimpleFeedForward(input,
                                                                  hidden1, hidden2, output, true);
            IMLDataSet training = RandomTrainingFactory.Generate(1000,
                                                                10000, input, output, -1, 1);

            return EvaluateTrain(network, training);
        }


        /// <summary>
        /// Evaluate how long it takes to calculate the error for the network. This
        /// causes each of the training pairs to be run through the network. The
        /// network is evaluated 10 times and the lowest time is reported. 
        /// </summary>
        /// <param name="network">The training data to use.</param>
        /// <param name="training">The number of seconds that it took.</param>
        /// <returns></returns>
        public static int EvaluateTrain(BasicNetwork network, IMLDataSet training)
        {
            // train the neural network
            IMLTrain train = new ResilientPropagation(network, training);

            int iterations = 0;
			const int milis10 = Milis * 10;
            var watch = new Stopwatch();
            watch.Start();
            while (true)
            {
                iterations++;
                train.Iteration();

				if((iterations & 0xff) == 0 && watch.ElapsedMilliseconds < milis10) break;
            }

            return iterations;
        }
    }
}
