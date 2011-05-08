// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Engine.Network.Train.Prop;
using Encog.Util.Simple;
using System.Diagnostics;
using Encog.Util.Time;

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
        public const int MILIS = 1000;

        /// <summary>
        /// Evaluate training.
        /// </summary>
        /// <param name="device">The OpenCL device, null for CPU.</param>
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
            MLDataSet training = RandomTrainingFactory.Generate(1000,
                    10000, input, output, -1, 1);

            return EvaluateTrain(network, training);
        }


        /// <summary>
        /// Evaluate how long it takes to calculate the error for the network. This
        /// causes each of the training pairs to be run through the network. The
        /// network is evaluated 10 times and the lowest time is reported. 
        /// </summary>
        /// <param name="profile">The network to evaluate with.</param>
        /// <param name="network">The training data to use.</param>
        /// <param name="training">The number of seconds that it took.</param>
        /// <returns></returns>
        public static int EvaluateTrain(BasicNetwork network, MLDataSet training)
        {
            // train the neural network
            ITrain train = train = new ResilientPropagation(network, training); 

            int iterations = 0;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (watch.ElapsedMilliseconds < (10 * MILIS))
            {
                iterations++;
                train.Iteration();
            }

            return iterations;
        }

    }
}
